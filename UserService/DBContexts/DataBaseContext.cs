using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Entities;

namespace UserService.DBContexts
{
    public class DataBaseContext : DbContext
    {
        public DbSet<User> User { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().HasKey(u => u.Id).HasName("PK_User");
            modelBuilder.Entity<User>().HasIndex(u => u.IdNo).IsUnique().HasDatabaseName("Idx_IdNo");
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique().HasDatabaseName("Idx_Email");


            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("Id").HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
            modelBuilder.Entity<User>().Property(u => u.IdNo).HasColumnName("IdNo").HasColumnType("varchar(9)").IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Email).HasColumnName("Email").HasColumnType("varchar(255)").IsRequired();
            modelBuilder.Entity<User>().Property(u => u.EngName).HasColumnName("EngName").HasColumnType("varchar(255)").IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Gender).HasColumnName("Gender").HasColumnType("varchar(1)").IsRequired();
        }
    }
}
