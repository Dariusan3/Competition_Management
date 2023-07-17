using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Management.Models;

[Table("CompetitionType")]
public partial class CompetitionType
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? TipName { get; set; }

    [InverseProperty("CompetitionTypeNavigation")]
    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();
}
