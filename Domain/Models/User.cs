using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[Index("Id", Name = "UQ__Users__3214EC26759E10CD", IsUnique = true)]
[Index("UserName", Name = "UQ__Users__C9F284563AAC2644", IsUnique = true)]
[Index("DeleteDate", "DeleteUserId", Name = "idx_ddate_duserId")]
public partial class User
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(255)]
    public string? FirstName { get; set; }

    [StringLength(255)]
    public string? LastName { get; set; }

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public long? CreateDate { get; set; }

    [Column("CreateUserID")]
    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    [Column("ModifyUserID")]
    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    [Column("DeleteUserID")]
    public long? DeleteUserId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<LogLogin> LogLogins { get; } = new List<LogLogin>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}
