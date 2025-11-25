using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("THUONGHIEU")]
[Index("Ten", Name = "UQ__THUONGHI__C451FA833D94F80E", IsUnique = true)]
public partial class Thuonghieu
{
    [Key]
    [Column("MaTH")]
    public int MaTh { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [InverseProperty("MaThNavigation")]
    public virtual ICollection<Mathang> Mathangs { get; set; } = new List<Mathang>();
}
