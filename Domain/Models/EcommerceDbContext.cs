using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class EcommerceDbContext : DbContext
{
    public EcommerceDbContext()
    {
    }

    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductBrand> ProductBrands { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;DataBase=EcommerceDB;Trusted_Connection=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.BrandId, "IX_Products_BrandId");

            entity.HasIndex(e => e.ProductTypeId, "IX_Products_ProductTypeId");

            entity.Property(e => e.BrandId).HasDefaultValueSql("(CONVERT([bigint],(0)))");
            entity.Property(e => e.Cdate).HasColumnName("CDate");
            entity.Property(e => e.CuserId).HasColumnName("CUserID");
            entity.Property(e => e.Ddate).HasColumnName("DDate");
            entity.Property(e => e.DuserId).HasColumnName("DUserID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductTypeId).HasDefaultValueSql("(CONVERT([bigint],(0)))");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products).HasForeignKey(d => d.BrandId);

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products).HasForeignKey(d => d.ProductTypeId);
        });

        modelBuilder.Entity<ProductBrand>(entity =>
        {
            entity.Property(e => e.Cdate).HasColumnName("CDate");
            entity.Property(e => e.CuserId).HasColumnName("CUserID");
            entity.Property(e => e.Ddate).HasColumnName("DDate");
            entity.Property(e => e.DuserId).HasColumnName("DUserID");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.Property(e => e.Cdate).HasColumnName("CDate");
            entity.Property(e => e.CuserId).HasColumnName("CUserID");
            entity.Property(e => e.Ddate).HasColumnName("DDate");
            entity.Property(e => e.DuserId).HasColumnName("DUserID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
