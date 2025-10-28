using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class branching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchEntitiesId",
                table: "PendingUserAccount",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "PendingUserAccount",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingUserAccount_Branches_BranchEntitiesId",
                table: "PendingUserAccount");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_PendingUserAccount_BranchEntitiesId",
                table: "PendingUserAccount");

            migrationBuilder.DropColumn(
                name: "BranchEntitiesId",
                table: "PendingUserAccount");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "PendingUserAccount");
        }
    }
}
