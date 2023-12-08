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

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleParent> RoleParents { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;DataBase=FaterTest;Trusted_Connection=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC2741A8A9D6");

            entity.HasIndex(e => e.Gcode, "UQ__Roles__978B75478FCBF655").IsUnique();

            entity.HasIndex(e => new { e.DeleteDate, e.DeleteUserId }, "idx_ddate_duserId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Gcode).HasColumnName("GCode");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<RoleParent>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.ParentId, e.RoleId }).HasName("PK__RolePare__E4AD83F9C3CB6525");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");

            entity.HasOne(d => d.Parent).WithMany(p => p.RoleParentParents)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleParent_ParentRole_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleParentRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleParents_Role_ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27438D4D9D");

            entity.HasIndex(e => e.Id, "UQ__Users__3214EC26759E10CD").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F284563AAC2644").IsUnique();

            entity.HasIndex(e => new { e.DeleteDate, e.DeleteUserId }, "idx_ddate_duserId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.MainRoleId).HasColumnName("MainRoleID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.MainRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.MainRoleId)
                .HasConstraintName("FK_Users_UserRoles");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC27A53E145F");

            entity.HasIndex(e => e.Id, "UQ__UserRole__3214EC26E13845A4").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles_ID");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
