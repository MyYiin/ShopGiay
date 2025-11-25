using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("TONKHO")]
[Index("MaMh", "MaMs", "MaKc", Name = "UQ__TONKHO__5770A70A3C9A8A51", IsUnique = true)]
[Index("Sku", Name = "UQ__TONKHO__CA1ECF0D1F5F9D69", IsUnique = true)]
public partial class Tonkho
{
    [Key]
    public int MaK { get; set; }

    public int MaMh { get; set; }

    [Column("MaMS")]
    public int MaMs { get; set; }

    [Column("MaKC")]
    public int MaKc { get; set; }

    [Column("SKU")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Sku { get; set; }

    public short? SoLuongTonKho { get; set; }

    [Column("GiaGocBT")]
    public int? GiaGocBt { get; set; }

    [Column("GiaBanBT")]
    public int? GiaBanBt { get; set; }

    [InverseProperty("MaKNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [ForeignKey("MaKc")]
    [InverseProperty("Tonkhos")]
    public virtual Kichco MaKcNavigation { get; set; } = null!;

    [ForeignKey("MaMh")]
    [InverseProperty("Tonkhos")]
    public virtual Mathang MaMhNavigation { get; set; } = null!;

    [ForeignKey("MaMs")]
    [InverseProperty("Tonkhos")]
    public virtual Mausac MaMsNavigation { get; set; } = null!;
}
