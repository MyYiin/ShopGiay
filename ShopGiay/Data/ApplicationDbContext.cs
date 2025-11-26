using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Models;
using System;
using System.Collections.Generic;

namespace ShopGiay.Data;

public partial class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chucvu> Chucvus { get; set; }

    public virtual DbSet<Cthoadon> Cthoadons { get; set; }

    public virtual DbSet<Cuahang> Cuahangs { get; set; }

    public virtual DbSet<Danhgium> Danhgia { get; set; }

    public virtual DbSet<Diachi> Diachis { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Kichco> Kichcos { get; set; }

    public virtual DbSet<Loaigiay> Loaigiays { get; set; }

    public virtual DbSet<Mathang> Mathangs { get; set; }

    public virtual DbSet<Mausac> Mausacs { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Thuonghieu> Thuonghieus { get; set; }

    public virtual DbSet<Tonkho> Tonkhos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Chỉ dùng connection string này nếu chưa được cấu hình từ Program.cs
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback connection string - thường không dùng đến vì đã config trong Program.cs
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=shop_giay;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chucvu>(entity =>
        {
            entity.HasKey(e => e.MaCv).HasName("PK__CHUCVU__27258E765E11D519");

            entity.Property(e => e.HeSo).HasDefaultValue(1.0);
        });

        modelBuilder.Entity<Cthoadon>(entity =>
        {
            entity.HasKey(e => e.MaCthd).HasName("PK__CTHOADON__1E4FA771F512215C");

            entity.Property(e => e.DonGia).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue((short)1);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Cthoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaHD__66603565");

            entity.HasOne(d => d.MaKNavigation).WithMany(p => p.Cthoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaK__6754599E");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.Cthoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaMh__68487DD7");
        });

        modelBuilder.Entity<Cuahang>(entity =>
        {
            entity.HasKey(e => e.MaCh).HasName("PK__CUAHANG__27258E00A1FDB92B");
        });

        modelBuilder.Entity<Danhgium>(entity =>
        {
            entity.HasKey(e => e.MaDg).HasName("PK__DANHGIA__27258660CC45A924");

            entity.Property(e => e.NgayDg).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Danhgia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DANHGIA__MaKH__6EF57B66");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.Danhgia).HasConstraintName("FK__DANHGIA__MaMh__6E01572D");
        });

        modelBuilder.Entity<Diachi>(entity =>
        {
            entity.HasKey(e => e.MaDc).HasName("PK__DIACHI__2725866443F55466");

            entity.Property(e => e.MacDinh).HasDefaultValue(1);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Diachis)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DIACHI__MaKH__5BE2A6F2");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HOADON__2725A6E002FDC379");

            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue(0);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Hoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HOADON__MaKH__619B8048");
 
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACHHAN__2725CF1E036AC910");
        });

        modelBuilder.Entity<Kichco>(entity =>
        {
            entity.HasKey(e => e.MaKc).HasName("PK__KICHCO__2725CF03E5446801");
        });

        modelBuilder.Entity<Loaigiay>(entity =>
        {
            entity.HasKey(e => e.MaLg).HasName("PK__LOAIGIAY__2725C77E89E2F000");
        });

        modelBuilder.Entity<Mathang>(entity =>
        {
            entity.HasKey(e => e.MaMh).HasName("PK__MATHANG__2725DF398F26F1C4");

            entity.Property(e => e.GiaBan).HasDefaultValue(0);
            entity.Property(e => e.GiaGoc).HasDefaultValue(0);
            entity.Property(e => e.LuotMua).HasDefaultValue(0);
            entity.Property(e => e.LuotXem).HasDefaultValue(0);

            entity.HasOne(d => d.MaLgNavigation).WithMany(p => p.Mathangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MATHANG__MaLG__44FF419A");

            entity.HasOne(d => d.MaThNavigation).WithMany(p => p.Mathangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MATHANG__MaTH__45F365D3");
        });

        modelBuilder.Entity<Mausac>(entity =>
        {
            entity.HasKey(e => e.MaMs).HasName("PK__MAUSAC__2725DFD64440CB33");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NHANVIEN__2725D70AA0F35AFF");

            entity.HasOne(d => d.MaCvNavigation).WithMany(p => p.Nhanviens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NHANVIEN__MaCV__571DF1D5");
        });

        modelBuilder.Entity<Thuonghieu>(entity =>
        {
            entity.HasKey(e => e.MaTh).HasName("PK__THUONGHI__27250075C1AF4225");
        });

        modelBuilder.Entity<Tonkho>(entity =>
        {
            entity.HasKey(e => e.MaK).HasName("PK__TONKHO__C7977BADD28F2A89");

            entity.Property(e => e.GiaBanBt).HasDefaultValue(0);
            entity.Property(e => e.GiaGocBt).HasDefaultValue(0);
            entity.Property(e => e.SoLuongTonKho).HasDefaultValue((short)0);

            entity.HasOne(d => d.MaKcNavigation).WithMany(p => p.Tonkhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TONKHO__MaKC__4E88ABD4");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.Tonkhos).HasConstraintName("FK__TONKHO__MaMh__4CA06362");

            entity.HasOne(d => d.MaMsNavigation).WithMany(p => p.Tonkhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TONKHO__MaMS__4D94879B");
        });
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
