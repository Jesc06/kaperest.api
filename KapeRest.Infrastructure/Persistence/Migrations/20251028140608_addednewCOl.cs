using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addednewCOl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingUserAccount_Branches_BranchEntitiesId",
                table: "PendingUserAccount");

            migrationBuilder.DropIndex(
                name: "IX_PendingUserAccount_BranchEntitiesId",
                table: "PendingUserAccount");

            migrationBuilder.DropColumn(
                name: "BranchEntitiesId",
                table: "PendingUserAccount");

            migrationBuilder.CreateIndex(
                name: "IX_PendingUserAccount_BranchId",
                table: "PendingUserAccount",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingUserAccount_Branches_BranchId",
                table: "PendingUserAccount",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingUserAccount_Branches_BranchId",
                table: "PendingUserAccount");

            migrationBuilder.DropIndex(
                name: "IX_PendingUserAccount_BranchId",
                table: "PendingUserAccount");

            migrationBuilder.AddColumn<int>(
                name: "BranchEntitiesId",
                table: "PendingUserAccount",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingUserAccount_BranchEntitiesId",
                table: "PendingUserAccount",
                column: "BranchEntitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingUserAccount_Branches_BranchEntitiesId",
                table: "PendingUserAccount",
                column: "BranchEntitiesId",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
