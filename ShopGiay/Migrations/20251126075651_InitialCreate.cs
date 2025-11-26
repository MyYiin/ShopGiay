using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopGiay.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CHUCVU",
                columns: table => new
                {
                    MaCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HeSo = table.Column<double>(type: "float", nullable: true, defaultValue: 1.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHUCVU__27258E765E11D519", x => x.MaCV);
                });

            migrationBuilder.CreateTable(
                name: "CUAHANG",
                columns: table => new
                {
                    MaCH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DienThoai = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CUAHANG__27258E00A1FDB92B", x => x.MaCH);
                });

            migrationBuilder.CreateTable(
                name: "KHACHHANG",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DienThoai = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KHACHHAN__2725CF1E036AC910", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "KICHCO",
                columns: table => new
                {
                    MaKC = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GiaTriKC = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KICHCO__2725CF03E5446801", x => x.MaKC);
                });

            migrationBuilder.CreateTable(
                name: "LOAIGIAY",
                columns: table => new
                {
                    MaLG = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LOAIGIAY__2725C77E89E2F000", x => x.MaLG);
                });

            migrationBuilder.CreateTable(
                name: "MAUSAC",
                columns: table => new
                {
                    MaMS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaHex = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MAUSAC__2725DFD64440CB33", x => x.MaMS);
                });

            migrationBuilder.CreateTable(
                name: "THUONGHIEU",
                columns: table => new
                {
                    MaTH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__THUONGHI__27250075C1AF4225", x => x.MaTH);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NHANVIEN",
                columns: table => new
                {
                    MaNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaCV = table.Column<int>(type: "int", nullable: false),
                    DienThoai = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MatKhau = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NHANVIEN__2725D70AA0F35AFF", x => x.MaNV);
                    table.ForeignKey(
                        name: "FK__NHANVIEN__MaCV__571DF1D5",
                        column: x => x.MaCV,
                        principalTable: "CHUCVU",
                        principalColumn: "MaCV");
                });

            migrationBuilder.CreateTable(
                name: "DIACHI",
                columns: table => new
                {
                    MaDC = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhuongXa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuanHuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MacDinh = table.Column<int>(type: "int", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DIACHI__2725866443F55466", x => x.MaDC);
                    table.ForeignKey(
                        name: "FK__DIACHI__MaKH__5BE2A6F2",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "HOADON",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TongTien = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HOADON__2725A6E002FDC379", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK__HOADON__MaKH__619B8048",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "MATHANG",
                columns: table => new
                {
                    MaMh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GiaGoc = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    GiaBan = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    MaLG = table.Column<int>(type: "int", nullable: false),
                    MaTH = table.Column<int>(type: "int", nullable: false),
                    HinhAnh = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    LuotXem = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    LuotMua = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MATHANG__2725DF398F26F1C4", x => x.MaMh);
                    table.ForeignKey(
                        name: "FK__MATHANG__MaLG__44FF419A",
                        column: x => x.MaLG,
                        principalTable: "LOAIGIAY",
                        principalColumn: "MaLG");
                    table.ForeignKey(
                        name: "FK__MATHANG__MaTH__45F365D3",
                        column: x => x.MaTH,
                        principalTable: "THUONGHIEU",
                        principalColumn: "MaTH");
                });

            migrationBuilder.CreateTable(
                name: "DANHGIA",
                columns: table => new
                {
                    MaDG = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaMh = table.Column<int>(type: "int", nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    Diem = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    NgayDG = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DANHGIA__27258660CC45A924", x => x.MaDG);
                    table.ForeignKey(
                        name: "FK__DANHGIA__MaKH__6EF57B66",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK__DANHGIA__MaMh__6E01572D",
                        column: x => x.MaMh,
                        principalTable: "MATHANG",
                        principalColumn: "MaMh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TONKHO",
                columns: table => new
                {
                    MaK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaMh = table.Column<int>(type: "int", nullable: false),
                    MaMS = table.Column<int>(type: "int", nullable: false),
                    MaKC = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SoLuongTonKho = table.Column<short>(type: "smallint", nullable: true, defaultValue: (short)0),
                    GiaGocBT = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    GiaBanBT = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TONKHO__C7977BADD28F2A89", x => x.MaK);
                    table.ForeignKey(
                        name: "FK__TONKHO__MaKC__4E88ABD4",
                        column: x => x.MaKC,
                        principalTable: "KICHCO",
                        principalColumn: "MaKC");
                    table.ForeignKey(
                        name: "FK__TONKHO__MaMS__4D94879B",
                        column: x => x.MaMS,
                        principalTable: "MAUSAC",
                        principalColumn: "MaMS");
                    table.ForeignKey(
                        name: "FK__TONKHO__MaMh__4CA06362",
                        column: x => x.MaMh,
                        principalTable: "MATHANG",
                        principalColumn: "MaMh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTHOADON",
                columns: table => new
                {
                    MaCTHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHD = table.Column<int>(type: "int", nullable: false),
                    MaK = table.Column<int>(type: "int", nullable: false),
                    MaMh = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    SoLuong = table.Column<short>(type: "smallint", nullable: true, defaultValue: (short)1),
                    ThanhTien = table.Column<int>(type: "int", nullable: true),
                    MaKc = table.Column<int>(type: "int", nullable: false),
                    MaMs = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CTHOADON__1E4FA771F512215C", x => x.MaCTHD);
                    table.ForeignKey(
                        name: "FK_CTHOADON_KICHCO_MaKc",
                        column: x => x.MaKc,
                        principalTable: "KICHCO",
                        principalColumn: "MaKC",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTHOADON_MAUSAC_MaMs",
                        column: x => x.MaMs,
                        principalTable: "MAUSAC",
                        principalColumn: "MaMS",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__CTHOADON__MaHD__66603565",
                        column: x => x.MaHD,
                        principalTable: "HOADON",
                        principalColumn: "MaHD");
                    table.ForeignKey(
                        name: "FK__CTHOADON__MaK__6754599E",
                        column: x => x.MaK,
                        principalTable: "TONKHO",
                        principalColumn: "MaK");
                    table.ForeignKey(
                        name: "FK__CTHOADON__MaMh__68487DD7",
                        column: x => x.MaMh,
                        principalTable: "MATHANG",
                        principalColumn: "MaMh");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaHD",
                table: "CTHOADON",
                column: "MaHD");

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaK",
                table: "CTHOADON",
                column: "MaK");

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaKc",
                table: "CTHOADON",
                column: "MaKc");

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaMh",
                table: "CTHOADON",
                column: "MaMh");

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaMs",
                table: "CTHOADON",
                column: "MaMs");

            migrationBuilder.CreateIndex(
                name: "IX_DANHGIA_MaMh",
                table: "DANHGIA",
                column: "MaMh");

            migrationBuilder.CreateIndex(
                name: "UQ__DANHGIA__A55792EC175A939E",
                table: "DANHGIA",
                columns: new[] { "MaKH", "MaMh" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DIACHI_MaKH",
                table: "DIACHI",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_HOADON_MaKH",
                table: "HOADON",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "UQ__KICHCO__27E03D26500303C4",
                table: "KICHCO",
                column: "GiaTriKC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MATHANG_MaLG",
                table: "MATHANG",
                column: "MaLG");

            migrationBuilder.CreateIndex(
                name: "IX_MATHANG_MaTH",
                table: "MATHANG",
                column: "MaTH");

            migrationBuilder.CreateIndex(
                name: "IX_NHANVIEN_MaCV",
                table: "NHANVIEN",
                column: "MaCV");

            migrationBuilder.CreateIndex(
                name: "UQ__THUONGHI__C451FA833D94F80E",
                table: "THUONGHIEU",
                column: "Ten",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TONKHO_MaKC",
                table: "TONKHO",
                column: "MaKC");

            migrationBuilder.CreateIndex(
                name: "IX_TONKHO_MaMS",
                table: "TONKHO",
                column: "MaMS");

            migrationBuilder.CreateIndex(
                name: "UQ__TONKHO__5770A70A3C9A8A51",
                table: "TONKHO",
                columns: new[] { "MaMh", "MaMS", "MaKC" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__TONKHO__CA1ECF0D1F5F9D69",
                table: "TONKHO",
                column: "SKU",
                unique: true,
                filter: "([SKU] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CTHOADON");

            migrationBuilder.DropTable(
                name: "CUAHANG");

            migrationBuilder.DropTable(
                name: "DANHGIA");

            migrationBuilder.DropTable(
                name: "DIACHI");

            migrationBuilder.DropTable(
                name: "NHANVIEN");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "HOADON");

            migrationBuilder.DropTable(
                name: "TONKHO");

            migrationBuilder.DropTable(
                name: "CHUCVU");

            migrationBuilder.DropTable(
                name: "KHACHHANG");

            migrationBuilder.DropTable(
                name: "KICHCO");

            migrationBuilder.DropTable(
                name: "MAUSAC");

            migrationBuilder.DropTable(
                name: "MATHANG");

            migrationBuilder.DropTable(
                name: "LOAIGIAY");

            migrationBuilder.DropTable(
                name: "THUONGHIEU");
        }
    }
}
