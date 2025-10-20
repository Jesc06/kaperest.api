using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PendingAccounts",
                table: "PendingAccounts");

            migrationBuilder.RenameTable(
                name: "PendingAccounts",
                newName: "PendingUserAccount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PendingUserAccount",
                table: "PendingUserAccount",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PendingUserAccount",
                table: "PendingUserAccount");

            migrationBuilder.RenameTable(
                name: "PendingUserAccount",
                newName: "PendingAccounts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PendingAccounts",
                table: "PendingAccounts",
                column: "Id");
        }
    }
}
