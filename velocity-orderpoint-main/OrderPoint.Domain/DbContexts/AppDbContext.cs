using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.ModifyIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.DbContexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
             : base(options)
        {
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerUser> CustomerUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLineItem> OrderLineItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Setting> Settings { get; set; }

        // FIX: Renamed from 'Users' to 'LegacyUsers' to avoid conflict with IdentityDbContext.Users
        public DbSet<User> LegacyUsers { get; set; }

        public DbSet<Wholesaler> Wholesalers { get; set; }
        public DbSet<WholesalerUser> WholesalerUsers { get; set; }
        public DbSet<Lists> List { get; set; }
        public DbSet<ListTypes> ListType { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<DeliveryOptions> DeliveryOptions { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<CustomerProducts> CustomerProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensures Identity configurations are applied

            modelBuilder.Entity<Category>()
           .HasKey(c => new { c.WholesalerID, c.ID });

            modelBuilder.Entity<CustomerUser>()
                .HasKey(cu => new { cu.UserID, cu.CustomerID });

            modelBuilder.Entity<WholesalerUser>()
                .HasKey(wu => new { wu.UserID, wu.WholesalerID });

            modelBuilder.Entity<Product>()
                .HasKey(p => new { p.WholesalerID, p.ID });

            modelBuilder.Entity<DeliveryOptions>(entity =>
            {
                entity.ToTable("tbDeliveryOptions");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.WholesalerID)
                    .IsRequired();

                entity.Property(e => e.WeekDay)
                    .HasMaxLength(20);
            });
        }
    }
}