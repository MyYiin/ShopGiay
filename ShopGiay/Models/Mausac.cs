using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("MAUSAC")]
public partial class Mausac
{
    [Key]
    [Column("MaMS")]
    public int MaMs { get; set; }

    [StringLength(50)]
    public string Ten { get; set; } = null!;

    [StringLength(7)]
    [Unicode(false)]
    public string? MaHex { get; set; }

    [InverseProperty("MaMsNavigation")]
    public virtual ICollection<Tonkho> Tonkhos { get; set; } = new List<Tonkho>();
}
