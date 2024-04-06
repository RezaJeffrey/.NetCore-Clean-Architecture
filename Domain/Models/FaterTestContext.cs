using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class FaterTestContext : DbContext
{
    public FaterTestContext(DbContextOptions<FaterTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Finance> Finances { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<LogLogin> LogLogins { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductRequest> ProductRequests { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectApplication> ProjectApplications { get; set; }

    public virtual DbSet<ProjectService> ProjectServices { get; set; }

    public virtual DbSet<ProjectServiceTask> ProjectServiceTasks { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<RegisterRequest> RegisterRequests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleParent> RoleParents { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceTask> ServiceTasks { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketMessage> TicketMessages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLocation> UserLocations { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Wflist> Wflists { get; set; }

    public virtual DbSet<Wfstatus> Wfstatuses { get; set; }

    public virtual DbSet<Wfstep> Wfsteps { get; set; }

    public virtual DbSet<Wftransition> Wftransitions { get; set; }

    public virtual DbSet<WorkFlow> WorkFlows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.Description).HasDefaultValueSql("('')");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.ProvinceId).HasColumnName("ProvinceID");

            entity.HasOne(d => d.Province).WithMany(p => p.Cities)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_Province");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.ToTable("File");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Url).IsUnicode(false);

            entity.HasOne(d => d.Entity).WithMany(p => p.Files)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("FK_Product");

            entity.HasOne(d => d.EntityNavigation).WithMany(p => p.Files)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("FK_Project");

            entity.HasOne(d => d.Entity1).WithMany(p => p.Files)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("FK_File_WFTransition");
        });

        modelBuilder.Entity<Finance>(entity =>
        {
            entity.ToTable("Finance");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.Description).HasDefaultValueSql("('')");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.WorkFlowId).HasColumnName("WorkFlowID");

            entity.HasOne(d => d.WorkFlow).WithMany(p => p.Finances)
                .HasForeignKey(d => d.WorkFlowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Finance_WorkFlow");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Location");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.Description).HasDefaultValueSql("('')");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Street).HasMaxLength(4000);
            entity.Property(e => e.Xgps)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("XGPS");
            entity.Property(e => e.Ygps)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("YGPS");
            entity.Property(e => e.ZipCode).HasMaxLength(255);

            entity.HasOne(d => d.City).WithMany(p => p.Locations)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Location_City");

            entity.HasOne(d => d.Region).WithMany(p => p.Locations)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Location_Region");
        });

        modelBuilder.Entity<LogLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_LogLogin_1");

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
                .HasConstraintName("FK_LogLogin_Users");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Title).HasMaxLength(300);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.HasIndex(e => e.Tcode, "UQ_Order_TCode").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Tcode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("TCode");
            entity.Property(e => e.TotalPrice)
                .HasDefaultValueSql("((0))")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WorkFlowId).HasColumnName("WorkFlowID");

            entity.HasOne(d => d.Location).WithMany(p => p.Orders)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_Order_Location");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Order_Users");

            entity.HasOne(d => d.WorkFlow).WithMany(p => p.Orders)
                .HasForeignKey(d => d.WorkFlowId)
                .HasConstraintName("FK_Order_WorkFlow");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.IsAvailable).HasDefaultValueSql("((0))");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TutorialUrl).HasColumnName("TutorialURL");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<ProductRequest>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ReceiptId).HasColumnName("ReceiptID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductRequests)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductRequests_Product");

            entity.HasOne(d => d.Receipt).WithMany(p => p.ProductRequests)
                .HasForeignKey(d => d.ReceiptId)
                .HasConstraintName("FK_ProductRequests_Receipt");

            entity.HasOne(d => d.User).WithMany(p => p.ProductRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductRequests_Users");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AgentId).HasColumnName("AgentID");
            entity.Property(e => e.Areas).HasMaxLength(4000);
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.CurrentStepGkey).HasDefaultValueSql("((0))");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.DistanceToRiser).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FinanceId).HasColumnName("FinanceID");
            entity.Property(e => e.LocationId)
                .HasDefaultValueSql("((0))")
                .HasColumnName("LocationID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.UnitArea).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WorkFlowId).HasColumnName("WorkFlowID");

            entity.HasOne(d => d.Agent).WithMany(p => p.ProjectAgents)
                .HasForeignKey(d => d.AgentId)
                .HasConstraintName("FK_Project_Users_Agent");

            entity.HasOne(d => d.CurrentStepGkeyNavigation).WithMany(p => p.Projects)
                .HasPrincipalKey(p => p.Gkey)
                .HasForeignKey(d => d.CurrentStepGkey)
                .HasConstraintName("FK_Project_WFStep");

            entity.HasOne(d => d.Finance).WithMany(p => p.Projects)
                .HasForeignKey(d => d.FinanceId)
                .HasConstraintName("FK_Project_Finance");

            entity.HasOne(d => d.Location).WithMany(p => p.Projects)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_Project_Location");

            entity.HasOne(d => d.Owner).WithMany(p => p.ProjectOwners)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Project_Users_Owner");

            entity.HasOne(d => d.WorkFlow).WithMany(p => p.Projects)
                .HasForeignKey(d => d.WorkFlowId)
                .HasConstraintName("FK_Project_WorkFlow");
        });

        modelBuilder.Entity<ProjectApplication>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

            entity.HasOne(d => d.Application).WithMany(p => p.ProjectApplications)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectApplications_Application");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectApplications)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectApplications_Project1");
        });

        modelBuilder.Entity<ProjectService>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectServices)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectServices_Project");

            entity.HasOne(d => d.Service).WithMany(p => p.ProjectServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectServices_Service");
        });

        modelBuilder.Entity<ProjectServiceTask>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.ServiceTaskId).HasColumnName("ServiceTaskID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectServiceTasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectServiceTasks_Project");

            entity.HasOne(d => d.ServiceTask).WithMany(p => p.ProjectServiceTasks)
                .HasForeignKey(d => d.ServiceTaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectServiceTasks_ServiceTask");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.ToTable("Province");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.ToTable("Receipt");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Category).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Receipt_Category");

            entity.HasOne(d => d.Order).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Receipt_Order");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.ToTable("Region");

            entity.HasIndex(e => new { e.RegCode, e.CityId }, "IX_Region_UQ_RegCode_CityID").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");

            entity.HasOne(d => d.City).WithMany(p => p.Regions)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Region_City");
        });

        modelBuilder.Entity<RegisterRequest>(entity =>
        {
            entity.ToTable("RegisterRequest");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RequestCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.RegisterRequests)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegisterRequest_Roles");

            entity.HasOne(d => d.Status).WithMany(p => p.RegisterRequests)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_RegisterRequest_Status");

            entity.HasOne(d => d.User).WithMany(p => p.RegisterRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegisterRequest_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__ID");

            entity.HasIndex(e => e.Gcode, "UQ__Roles__978B75478FCBF655").IsUnique();

            entity.HasIndex(e => new { e.DeleteDate, e.DeleteUserId }, "idx_ddate_duserId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
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

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ServiceTask>(entity =>
        {
            entity.ToTable("ServiceTask");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_ServiceTask_ServiceTask");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.HasIndex(e => e.Gkey, "IX_Status_UQ_Gkey").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subject");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.ReceiveRoleId).HasColumnName("ReceiveRoleID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Title).HasMaxLength(300);

            entity.HasOne(d => d.CreateUser).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CreateUserId)
                .HasConstraintName("FK_Ticket_Users");

            entity.HasOne(d => d.ReceiveRole).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ReceiveRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Roles");

            entity.HasOne(d => d.Status).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Status");

            entity.HasOne(d => d.Subject).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Subject");
        });

        modelBuilder.Entity<TicketMessage>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Body).HasDefaultValueSql("('')");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.TicketId).HasColumnName("TicketID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketMessages)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketMessages_Ticket");

            entity.HasOne(d => d.User).WithMany(p => p.TicketMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketMessages_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27438D4D9D");

            entity.HasIndex(e => e.Id, "UQ__Users__3214EC26759E10CD").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F284563AAC2644").IsUnique();

            entity.HasIndex(e => new { e.DeleteDate, e.DeleteUserId }, "idx_ddate_duserId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccessCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.Property(e => e.UserStatusId).HasColumnName("UserStatusID");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Users_Users_Parent");

            entity.HasOne(d => d.UserStatus).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserStatusId)
                .HasConstraintName("FK_Users_Status");
        });

        modelBuilder.Entity<UserLocation>(entity =>
        {
            entity.HasIndex(e => e.LocationId, "IX_UserLocations").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.LocationId)
                .HasDefaultValueSql("('')")
                .HasColumnName("LocationID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Location).WithOne(p => p.UserLocation)
                .HasForeignKey<UserLocation>(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserLocations_Location");

            entity.HasOne(d => d.User).WithMany(p => p.UserLocations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserLocations_Users");
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

        modelBuilder.Entity<Wflist>(entity =>
        {
            entity.ToTable("WFList");

            entity.HasIndex(e => e.Gkey, "IX_WFList_UQ_Gkey").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<Wfstatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WFStatus_1");

            entity.ToTable("WFStatus");

            entity.HasIndex(e => e.Gkey, "IX_WFStatus_UQ_Gkey").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Wfstep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WFStatus");

            entity.ToTable("WFStep");

            entity.HasIndex(e => e.Gkey, "IX_WFStep_UQ_Gkey").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasDefaultValueSql("('')");
            entity.Property(e => e.WflistId).HasColumnName("WFListID");

            entity.HasOne(d => d.Wflist).WithMany(p => p.Wfsteps)
                .HasForeignKey(d => d.WflistId)
                .HasConstraintName("FK_WFStep_WFList");
        });

        modelBuilder.Entity<Wftransition>(entity =>
        {
            entity.ToTable("WFTransition");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.CurrentStepId).HasColumnName("CurrentStepID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.FromStepId)
                .HasDefaultValueSql("('')")
                .HasColumnName("FromStepID");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.TransitionDate).HasDefaultValueSql("('')");
            entity.Property(e => e.WfstatusId).HasColumnName("WFStatusID");
            entity.Property(e => e.WorkFlowId).HasColumnName("WorkFlowID");

            entity.HasOne(d => d.CurrentStep).WithMany(p => p.WftransitionCurrentSteps)
                .HasForeignKey(d => d.CurrentStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WFTransition_WFStep_To");

            entity.HasOne(d => d.FromStep).WithMany(p => p.WftransitionFromSteps)
                .HasForeignKey(d => d.FromStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WFTransition_WFStep");

            entity.HasOne(d => d.Wfstatus).WithMany(p => p.Wftransitions)
                .HasForeignKey(d => d.WfstatusId)
                .HasConstraintName("FK_WFTransition_WFStatusID");

            entity.HasOne(d => d.WorkFlow).WithMany(p => p.Wftransitions)
                .HasForeignKey(d => d.WorkFlowId)
                .HasConstraintName("FK_WFTransition_WorkFlow");
        });

        modelBuilder.Entity<WorkFlow>(entity =>
        {
            entity.ToTable("WorkFlow");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateUserId).HasColumnName("CreateUserID");
            entity.Property(e => e.DeleteUserId).HasColumnName("DeleteUserID");
            entity.Property(e => e.Description).HasDefaultValueSql("('')");
            entity.Property(e => e.ModifyUserId).HasColumnName("ModifyUserID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasDefaultValueSql("('')");
            entity.Property(e => e.WflistId).HasColumnName("WFListID");

            entity.HasOne(d => d.Wflist).WithMany(p => p.WorkFlows)
                .HasForeignKey(d => d.WflistId)
                .HasConstraintName("FK_WorkFlow_WFList");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
