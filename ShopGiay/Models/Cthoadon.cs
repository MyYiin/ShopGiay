using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("CTHOADON")]
[Index("MaHd", Name = "IX_CTHOADON_MaHD")]
[Index("MaK", Name = "IX_CTHOADON_MaK")]
[Index("MaKc", Name = "IX_CTHOADON_MaKc")]
[Index("MaMh", Name = "IX_CTHOADON_MaMh")]
[Index("MaMs", Name = "IX_CTHOADON_MaMs")]
public partial class Cthoadon
{
    [Key]
    [Column("MaCTHD")]
    public int MaCthd { get; set; }

    [Column("MaHD")]
    public int MaHd { get; set; }

    public int MaK { get; set; }

    public int MaMh { get; set; }

    public int? DonGia { get; set; }

    public short? SoLuong { get; set; }

    public int? ThanhTien { get; set; }

    public int MaKc { get; set; }

    public int MaMs { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("Cthoadons")]
    public virtual Hoadon MaHdNavigation { get; set; } = null!;

    [ForeignKey("MaK")]
    [InverseProperty("Cthoadons")]
    public virtual Tonkho MaKNavigation { get; set; } = null!;

    [ForeignKey("MaKc")]
    [InverseProperty("Cthoadons")]
    public virtual Kichco MaKcNavigation { get; set; } = null!;

    [ForeignKey("MaMh")]
    [InverseProperty("Cthoadons")]
    public virtual Mathang MaMhNavigation { get; set; } = null!;

    [ForeignKey("MaMs")]
    [InverseProperty("Cthoadons")]
    public virtual Mausac MaMsNavigation { get; set; } = null!;
}
