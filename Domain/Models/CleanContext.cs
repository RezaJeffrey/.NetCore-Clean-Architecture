using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class CleanContext : DbContext
{
    public CleanContext(DbContextOptions<CleanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LogLogin> LogLogins { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogLogin>(entity =>
        {
            entity.ToTable("LogLogin");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.LogLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogLogin_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasIndex(e => e.Gcode, "UQ_Role_Gcode").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.UserName, "UQ_User_UserName").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
