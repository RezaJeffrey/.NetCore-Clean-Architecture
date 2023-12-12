using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[Index("Gcode", Name = "UQ__Roles__978B75478FCBF655", IsUnique = true)]
[Index("DeleteDate", "DeleteUserId", Name = "idx_ddate_duserId")]
public partial class Role
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("GCode")]
    public int Gcode { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    public long? CreateDate { get; set; }

    [Column("CreateUserID")]
    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    [Column("ModifyUserID")]
    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    [Column("DeleteUserID")]
    public long? DeleteUserId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<RoleParent> RoleParentParents { get; } = new List<RoleParent>();

    [InverseProperty("Role")]
    public virtual ICollection<RoleParent> RoleParentRoles { get; } = new List<RoleParent>();

    [InverseProperty("Role")]
    public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}
