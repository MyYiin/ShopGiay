using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("KICHCO")]
[Index("GiaTriKc", Name = "UQ__KICHCO__27E03D26500303C4", IsUnique = true)]
public partial class Kichco
{
    [Key]
    [Column("MaKC")]
    public int MaKc { get; set; }

    [Column("GiaTriKC")]
    public double GiaTriKc { get; set; }

    [InverseProperty("MaKcNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [InverseProperty("MaKcNavigation")]
    public virtual ICollection<Tonkho> Tonkhos { get; set; } = new List<Tonkho>();
}
