using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopGiay.Migrations
{
    /// <inheritdoc />
    public partial class adddanhgia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrangThai",
                table: "HOADON",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "DANHGIA",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "DANHGIA");

            migrationBuilder.AlterColumn<int>(
                name: "TrangThai",
                table: "HOADON",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
