using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("NHANVIEN")]
[Index("MaCv", Name = "IX_NHANVIEN_MaCV")]
public partial class Nhanvien
{
    [Key]
    [Column("MaNV")]
    public int MaNv { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [Column("MaCV")]
    public int MaCv { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? MatKhau { get; set; }

    [ForeignKey("MaCv")]
    [InverseProperty("Nhanviens")]
    public virtual Chucvu MaCvNavigation { get; set; } = null!;
}
