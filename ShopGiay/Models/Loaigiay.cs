using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("LOAIGIAY")]
public partial class Loaigiay
{
    [Key]
    [Column("MaLG")]
    public int MaLg { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [InverseProperty("MaLgNavigation")]
    public virtual ICollection<Mathang> Mathangs { get; set; } = new List<Mathang>();
}
