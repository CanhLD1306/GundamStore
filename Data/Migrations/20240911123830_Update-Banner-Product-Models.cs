using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GundamStore.Data.Migrations
{
    public partial class UpdateBannerProductModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Banners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: true);
            }
    }
}
