using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Enums;

namespace ShopGiay.Models;

[Table("HOADON")]
[Index("MaKh", Name = "IX_HOADON_MaKH")]
public partial class Hoadon
{
    [Key]
    [Column("MaHD")]
    public int MaHd { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Ngay { get; set; }

    public int? TongTien { get; set; }

    [Column("MaKH")]
    public int MaKh { get; set; }

    public Status TrangThai { get; set; } = Status.ChoXuLy;

    //[StringLength(100)]
    //public string TenNguoiNhan { get; set; } = string.Empty;

    //[StringLength(255)]
    //public string DiaChiGiaoHang { get; set; } = string.Empty;

    //[StringLength(20)]
    //public string DienThoai { get; set; } = string.Empty;

    //public string GhiChu { get; set; } = string.Empty;

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [ForeignKey("MaKh")]
    [InverseProperty("Hoadons")]
    public virtual Khachhang MaKhNavigation { get; set; } = null!;

    public enum Status
    {
        ChoXuLy = 0,
        DaXacNhan = 1,
        DangGiaoHang = 2,
        HoanThanh = 3,
        DaHuy = 4
    }
}
