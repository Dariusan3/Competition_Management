using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Management.Models;

[Table("Competition")]
public partial class Competition
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    public int? CompetitionType { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Required]
    public string? CompetitionName { get; set; }

    [Column(TypeName = "date")]
    [Required]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "date")]
    [Required]
    public DateTime? EndDate { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Required]
    public string? Location { get; set; }

    public byte[]? Logo { get; set; }

    [ForeignKey("CompetitionType")]
    [InverseProperty("Competitions")]
    public virtual CompetitionType? CompetitionTypeNavigation { get; set; }

    [InverseProperty("Competition")]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    [ForeignKey("CompetitionId")]
    [InverseProperty("Competitions")]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
