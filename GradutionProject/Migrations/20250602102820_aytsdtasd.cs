using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradutionProject.Migrations
{
    /// <inheritdoc />
    public partial class aytsdtasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "College",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "CollegeId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearOfStudy",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CollegeId",
                table: "Admins",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "College",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearOfStudy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_College", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Professsor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professsor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Professsor_College_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "College",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Lecture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfLectures = table.Column<int>(type: "int", nullable: false),
                    Pdf = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: true),
                    ProfessorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lecture_College_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "College",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lecture_Professsor_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professsor",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "CollegeId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Students_CollegeId",
                table: "Students",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_CollegeId",
                table: "Admins",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_CollegeId",
                table: "Lecture",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_ProfessorId",
                table: "Lecture",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Professsor_CollegeId",
                table: "Professsor",
                column: "CollegeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_College_CollegeId",
                table: "Admins",
                column: "CollegeId",
                principalTable: "College",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_College_CollegeId",
                table: "Students",
                column: "CollegeId",
                principalTable: "College",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_College_CollegeId",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_College_CollegeId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "Lecture");

            migrationBuilder.DropTable(
                name: "Professsor");

            migrationBuilder.DropTable(
                name: "College");

            migrationBuilder.DropIndex(
                name: "IX_Students_CollegeId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Admins_CollegeId",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "YearOfStudy",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "Admins");

            migrationBuilder.AddColumn<string>(
                name: "College",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
