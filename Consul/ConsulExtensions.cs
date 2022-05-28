

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                
            }));





        }
    }
}
