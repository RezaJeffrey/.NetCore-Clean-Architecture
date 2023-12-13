using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class FaterTestContext : DbContext
{
    public FaterTestContext()
    {
    }

    public FaterTestContext(DbContextOptions<FaterTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LogLogin> LogLogins { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleParent> RoleParents { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;DataBase=FaterTest;Trusted_Connection=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_LogLogin_1");

            entity.HasOne(d => d.User).WithMany(p => p.LogLogins)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogLogin_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC2741A8A9D6");
        });

        modelBuilder.Entity<RoleParent>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.ParentId, e.RoleId }).HasName("PK__RolePare__E4AD83F9C3CB6525");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Parent).WithMany(p => p.RoleParentParents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleParent_ParentRole_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleParentRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleParents_Role_ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27438D4D9D");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC27A53E145F");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles_ID");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
