using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updatedDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "Products",
                newName: "ImageName");

            migrationBuilder.RenameColumn(
                name: "ImageData",
                table: "Products",
                newName: "ImageBase64");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Products",
                newName: "ImageURL");

            migrationBuilder.RenameColumn(
                name: "ImageBase64",
                table: "Products",
                newName: "ImageData");
        }
    }
}
