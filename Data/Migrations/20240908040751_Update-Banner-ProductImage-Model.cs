using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GundamStore.Data.Migrations
{
    public partial class UpdateBannerProductImageModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "ProductImages",
                newName: "Id");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Banners");
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Banners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "ProductImages",
                newName: "Id");
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Banners");
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Banners");
        }
    }
}
