using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradutionProject.Migrations
{
    /// <inheritdoc />
    public partial class asgfs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Colleges_CollegeId",
                table: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_Admins_CollegeId",
                table: "Admins");

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Colleges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colleges_AdminId",
                table: "Colleges",
                column: "AdminId",
                unique: true,
                filter: "[AdminId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_Admins_AdminId",
                table: "Colleges",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_Admins_AdminId",
                table: "Colleges");

            migrationBuilder.DropIndex(
                name: "IX_Colleges_AdminId",
                table: "Colleges");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Colleges");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_CollegeId",
                table: "Admins",
                column: "CollegeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Colleges_CollegeId",
                table: "Admins",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");
        }
    }
}
