using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[PrimaryKey("Id", "ParentId", "RoleId")]
public partial class RoleParent
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Key]
    [Column("RoleID")]
    public long RoleId { get; set; }

    [Key]
    [Column("ParentID")]
    public long ParentId { get; set; }

    public long? CreateDate { get; set; }

    [Column("CreateUserID")]
    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    [Column("ModifyUserID")]
    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    [Column("DeleteUserID")]
    public long? DeleteUserId { get; set; }

    [ForeignKey("ParentId")]
    [InverseProperty("RoleParentParents")]
    public virtual Role Parent { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("RoleParentRoles")]
    public virtual Role Role { get; set; } = null!;
}
