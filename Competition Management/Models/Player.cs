using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Management.Models;

[Table("Player")]
public partial class Player
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("TeamID")]
    public int? TeamId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? LastName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? FirstName { get; set; }

    public int? Age { get; set; }

    public byte[]? Photo { get; set; }

    [ForeignKey("TeamId")]
    [InverseProperty("Players")]
    public virtual Team? Team { get; set; }
}
