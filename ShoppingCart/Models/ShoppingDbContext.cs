using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Models;

public partial class ShoppingDbContext : DbContext
{
    public ShoppingDbContext()
    {
    }

    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<CartUser> CartUsers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-7156J9OV\\SQLEXPRESS;Database=ShoppingDB;uid=student;pwd=student;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Cart");

            entity.ToTable("CartItem");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CartUser>(entity =>
        {
            entity.ToTable("CartUser");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password).HasColumnType("text");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
