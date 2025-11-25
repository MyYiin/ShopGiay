using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopGiay.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTonKhoFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaKc",
                table: "CTHOADON",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaMs",
                table: "CTHOADON",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaKc",
                table: "CTHOADON",
                column: "MaKc");

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaMs",
                table: "CTHOADON",
                column: "MaMs");

            migrationBuilder.AddForeignKey(
                name: "FK_CTHOADON_KICHCO_MaKc",
                table: "CTHOADON",
                column: "MaKc",
                principalTable: "KICHCO",
                principalColumn: "MaKC",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTHOADON_MAUSAC_MaMs",
                table: "CTHOADON",
                column: "MaMs",
                principalTable: "MAUSAC",
                principalColumn: "MaMS",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTHOADON_KICHCO_MaKc",
                table: "CTHOADON");

            migrationBuilder.DropForeignKey(
                name: "FK_CTHOADON_MAUSAC_MaMs",
                table: "CTHOADON");

            migrationBuilder.DropIndex(
                name: "IX_CTHOADON_MaKc",
                table: "CTHOADON");

            migrationBuilder.DropIndex(
                name: "IX_CTHOADON_MaMs",
                table: "CTHOADON");

            migrationBuilder.DropColumn(
                name: "MaKc",
                table: "CTHOADON");

            migrationBuilder.DropColumn(
                name: "MaMs",
                table: "CTHOADON");
        }
    }
}
