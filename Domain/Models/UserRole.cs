using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[Index("Id", Name = "UQ__UserRole__3214EC26E13845A4", IsUnique = true)]
public partial class UserRole
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("UserID")]
    public long UserId { get; set; }

    [Column("RoleID")]
    public long RoleId { get; set; }

    public bool IsMainRole { get; set; }

    public long? CreateDate { get; set; }

    [Column("CreateUserID")]
    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    [Column("ModifyUserID")]
    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    [Column("DeleteUserID")]
    public long? DeleteUserId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserRoles")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserRoles")]
    public virtual User User { get; set; } = null!;
}
