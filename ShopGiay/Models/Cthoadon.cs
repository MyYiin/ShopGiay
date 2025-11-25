using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("CTHOADON")]
public partial class Cthoadon
{
    [Key]
    [Column("MaCTHD")]
    public int MaCthd { get; set; }

    [Column("MaHD")]
    public int MaHd { get; set; }

    public int MaK { get; set; }

    public int MaMh { get; set; }
    public int MaMs { get; set; }
    public int MaKc { get; set; }

    public int? DonGia { get; set; }

    public short? SoLuong { get; set; }

    public int? ThanhTien { get; set; }

    [ForeignKey("MaMs")]
    public virtual Mausac MaMsNavigation { get; set; }

    [ForeignKey("MaKc")]
    public virtual Kichco MaKcNavigation { get; set; }
    [ForeignKey("MaHd")]
    [InverseProperty("Cthoadons")]
    public virtual Hoadon MaHdNavigation { get; set; } = null!;

    [ForeignKey("MaK")]
    [InverseProperty("Cthoadons")]
    public virtual Tonkho MaKNavigation { get; set; } = null!;

    [ForeignKey("MaMh")]
    [InverseProperty("Cthoadons")]
    public virtual Mathang MaMhNavigation { get; set; } = null!;
}
