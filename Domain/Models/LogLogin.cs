using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[Table("LogLogin")]
public partial class LogLogin
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("UserID")]
    public long UserId { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string? IpAddress { get; set; }

    public long ExpDate { get; set; }

    public bool IsSuccess { get; set; }

    public long? CreateDate { get; set; }

    [Column("CreateUserID")]
    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    [Column("ModifyUserID")]
    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    [Column("DeleteUserID")]
    public long? DeleteUserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("LogLogins")]
    public virtual User User { get; set; } = null!;
}
