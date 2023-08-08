using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SaleOnline.Models;

//public partial class SaleOnline1Context : DbContext
public partial class SaleOnline1Context : IdentityDbContext
{
    public SaleOnline1Context()
    {
    }

    public SaleOnline1Context(DbContextOptions<SaleOnline1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    //public virtual DbSet<Role> Roles { get; set; }

    //public virtual DbSet<User> Users { get; set; }

    public new virtual DbSet<Role> Roles { get; set; }
    public new virtual DbSet<User> Users { get; set; }


    public DbSet<Customer> Customers { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("name=ketnoi");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("name=ketnoi"); // Cấu hình kết nối với SQL Server
        optionsBuilder.UseLazyLoadingProxies(); // Bật lazy-loading
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.Property(e => e.ProductId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CategoryId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders).HasConstraintName("FK_Orders_OrderStatus");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Payments");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Promotions");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(e => e.OrderItemId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems).HasConstraintName("FK_OrderItems_Products");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.Property(e => e.OrderStatusId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.PaymentId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Products).HasConstraintName("FK_Products_Promotions");

            entity.HasOne(d => d.User).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Products_Users");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.Property(e => e.PromotionId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Product).WithMany(p => p.Promotions).HasConstraintName("FK_Promotions_Products");

            entity.HasOne(d => d.User).WithMany(p => p.Promotions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Promotions_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
