using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GundamStore.Data.Migrations
{
    public partial class UpdateBannerCategoryScaleProductmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "OrderItems",
                newName: "Updated_By");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "OrderItems",
                newName: "Updated_At");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "OrderItems",
                newName: "Created_By");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OrderItems",
                newName: "Created_At");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Updated_By",
                table: "OrderItems",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "Updated_At",
                table: "OrderItems",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Created_By",
                table: "OrderItems",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "OrderItems",
                newName: "CreatedAt");
        }
    }
}
