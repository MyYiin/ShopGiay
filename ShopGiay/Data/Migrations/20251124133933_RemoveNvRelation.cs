using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopGiay.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNvRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__HOADON__MaNV__628FA481",
                table: "HOADON");

            migrationBuilder.DropIndex(
                name: "IX_HOADON_MaNV",
                table: "HOADON");

            migrationBuilder.DropColumn(
                name: "MaNV",
                table: "HOADON");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaNV",
                table: "HOADON",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HOADON_MaNV",
                table: "HOADON",
                column: "MaNV");

            migrationBuilder.AddForeignKey(
                name: "FK__HOADON__MaNV__628FA481",
                table: "HOADON",
                column: "MaNV",
                principalTable: "NHANVIEN",
                principalColumn: "MaNV");
        }
    }
}
