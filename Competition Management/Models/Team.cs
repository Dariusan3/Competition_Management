using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Management.Models;

[Table("Team")]
public partial class Team
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Required]
    public string? TeamName { get; set; }
    
    [Range(1, 9999)]
    public int? AwardNr { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Required]
    public string? Motto { get; set; }

    [Column(TypeName = "date")]
    [Required]
    public DateTime? CreatedOn { get; set; }

    public byte[]? Sigla { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Required]
    public string? Stadium { get; set; }

    [InverseProperty("Team1")]
    public virtual ICollection<Game> GameTeam1s { get; set; } = new List<Game>();

    [InverseProperty("Team2")]
    public virtual ICollection<Game> GameTeam2s { get; set; } = new List<Game>();

    [InverseProperty("Team")]
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    [ForeignKey("TeamId")]
    [InverseProperty("Teams")]
    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();
}
