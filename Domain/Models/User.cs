using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class User
{
    public long Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? AccessCode { get; set; }

    public long? ParentId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public long? UserStatusId { get; set; }

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual ICollection<User> InverseParent { get; set; } = new List<User>();

    public virtual ICollection<LogLogin> LogLogins { get; set; } = new List<LogLogin>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? Parent { get; set; }

    public virtual ICollection<ProductRequest> ProductRequests { get; set; } = new List<ProductRequest>();

    public virtual ICollection<Project> ProjectAgents { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectOwners { get; set; } = new List<Project>();

    public virtual ICollection<RegisterRequest> RegisterRequests { get; set; } = new List<RegisterRequest>();

    public virtual ICollection<TicketMessage> TicketMessages { get; set; } = new List<TicketMessage>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<UserLocation> UserLocations { get; set; } = new List<UserLocation>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual Status? UserStatus { get; set; }
}
