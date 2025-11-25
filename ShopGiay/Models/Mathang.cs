using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("MATHANG")]
public partial class Mathang
{
    [Key]
    public int MaMh { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [StringLength(1000)]
    public string? MoTa { get; set; }

    public int? GiaGoc { get; set; }

    public int? GiaBan { get; set; }

    [Column("MaLG")]
    public int MaLg { get; set; }

    [Column("MaTH")]
    public int MaTh { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? HinhAnh { get; set; }

    public int? LuotXem { get; set; }

    public int? LuotMua { get; set; }

    [InverseProperty("MaMhNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [InverseProperty("MaMhNavigation")]
    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    [ForeignKey("MaLg")]
    [InverseProperty("Mathangs")]
    public virtual Loaigiay MaLgNavigation { get; set; } = null!;

    [ForeignKey("MaTh")]
    [InverseProperty("Mathangs")]
    public virtual Thuonghieu MaThNavigation { get; set; } = null!;

    [InverseProperty("MaMhNavigation")]
    public virtual ICollection<Tonkho> Tonkhos { get; set; } = new List<Tonkho>();
}
