using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consul
{
    public static class ConsulExtensions
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new ConsulOptions();
            configuration.GetSection(nameof(ConsulOptions)).Bind(options);
            services.Configure<ConsulOptions>(configuration.GetSection(nameof(ConsulOptions)));

            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig => {
                var address = options.ConsulAddress;
                consulConfig.Address = new Uri(address);
            }));

            return services;

        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            // Retrive Consul client from DI
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulOptions>>();

            // setup logger
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            // get server IP address
            var address = consulConfig.Value.ServiceAddress;

            if (string.IsNullOrEmpty(address))
            {
                throw new Exception("cannot find consul service address in config");
            }

            // register service in consul
            var uri = new Uri(address);
            var serverName = consulConfig.Value.Name ?? AppDomain.CurrentDomain.FriendlyName.Trim().Trim('_');
            var registration = new AgentServiceRegistration
            {
                ID = $"{serverName.ToLowerInvariant()} - {consulConfig.Value.Id ?? Guid.NewGuid().ToString()}",
                Name = serverName,
                Address = uri.Host,
                Port = uri.Port,
                Tags = consulConfig.Value.Tags
            };

            if (!consulConfig.Value.DisableAgentCheck)
            {
                registration.Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(uri, "health").OriginalString
                };
            }

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(()=> {
                logger.LogInformation("Deregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
