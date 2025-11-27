using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models;

[Table("DANHGIA")]
[Index("MaMh", Name = "IX_DANHGIA_MaMh")]
[Index("MaKh", "MaMh", Name = "UQ__DANHGIA__A55792EC175A939E", IsUnique = true)]
public partial class Danhgia
{
    [Key]
    [Column("MaDG")]
    public int MaDg { get; set; }

    public int MaMh { get; set; }
    [Column("IsAnonymous")]
    public bool IsAnonymous { get; set; } = false;

    [Column("MaKH")]
    public int MaKh { get; set; }

    public int Diem { get; set; }

    [Column(TypeName = "text")]
    public string? NoiDung { get; set; }

    [Column("NgayDG", TypeName = "datetime")]
    public DateTime? NgayDg { get; set; }

    [ForeignKey("MaKh")]
    [InverseProperty("Danhgia")]
    public virtual Khachhang MaKhNavigation { get; set; } = null!;

    [ForeignKey("MaMh")]
    [InverseProperty("Danhgia")]
    public virtual Mathang MaMhNavigation { get; set; } = null!;
}
