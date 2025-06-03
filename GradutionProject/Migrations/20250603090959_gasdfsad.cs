using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradutionProject.Migrations
{
    /// <inheritdoc />
    public partial class gasdfsad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_College_CollegeId",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecture_College_CollegeId",
                table: "Lecture");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecture_Professsor_ProfessorId",
                table: "Lecture");

            migrationBuilder.DropForeignKey(
                name: "FK_Professsor_College_CollegeId",
                table: "Professsor");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_College_CollegeId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Professsor",
                table: "Professsor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecture",
                table: "Lecture");

            migrationBuilder.DropPrimaryKey(
                name: "PK_College",
                table: "College");

            migrationBuilder.RenameTable(
                name: "Professsor",
                newName: "Professsors");

            migrationBuilder.RenameTable(
                name: "Lecture",
                newName: "Lectures");

            migrationBuilder.RenameTable(
                name: "College",
                newName: "Colleges");

            migrationBuilder.RenameIndex(
                name: "IX_Professsor_CollegeId",
                table: "Professsors",
                newName: "IX_Professsors_CollegeId");

            migrationBuilder.RenameIndex(
                name: "IX_Lecture_ProfessorId",
                table: "Lectures",
                newName: "IX_Lectures_ProfessorId");

            migrationBuilder.RenameIndex(
                name: "IX_Lecture_CollegeId",
                table: "Lectures",
                newName: "IX_Lectures_CollegeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Professsors",
                table: "Professsors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lectures",
                table: "Lectures",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colleges",
                table: "Colleges",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DegreeOfLabs = table.Column<int>(type: "int", nullable: false),
                    DegreeOfStudiom = table.Column<int>(type: "int", nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: true),
                    ProfessorId = table.Column<int>(type: "int", nullable: true),
                    ProfesssorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courses_Professsors_ProfesssorId",
                        column: x => x.ProfesssorId,
                        principalTable: "Professsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CollegeId",
                table: "Courses",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ProfesssorId",
                table: "Courses",
                column: "ProfesssorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Colleges_CollegeId",
                table: "Admins",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Colleges_CollegeId",
                table: "Lectures",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Professsors_ProfessorId",
                table: "Lectures",
                column: "ProfessorId",
                principalTable: "Professsors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Professsors_Colleges_CollegeId",
                table: "Professsors",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Colleges_CollegeId",
                table: "Students",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Colleges_CollegeId",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Colleges_CollegeId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Professsors_ProfessorId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_Professsors_Colleges_CollegeId",
                table: "Professsors");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Colleges_CollegeId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Professsors",
                table: "Professsors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lectures",
                table: "Lectures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colleges",
                table: "Colleges");

            migrationBuilder.RenameTable(
                name: "Professsors",
                newName: "Professsor");

            migrationBuilder.RenameTable(
                name: "Lectures",
                newName: "Lecture");

            migrationBuilder.RenameTable(
                name: "Colleges",
                newName: "College");

            migrationBuilder.RenameIndex(
                name: "IX_Professsors_CollegeId",
                table: "Professsor",
                newName: "IX_Professsor_CollegeId");

            migrationBuilder.RenameIndex(
                name: "IX_Lectures_ProfessorId",
                table: "Lecture",
                newName: "IX_Lecture_ProfessorId");

            migrationBuilder.RenameIndex(
                name: "IX_Lectures_CollegeId",
                table: "Lecture",
                newName: "IX_Lecture_CollegeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Professsor",
                table: "Professsor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecture",
                table: "Lecture",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_College",
                table: "College",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_College_CollegeId",
                table: "Admins",
                column: "CollegeId",
                principalTable: "College",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecture_College_CollegeId",
                table: "Lecture",
                column: "CollegeId",
                principalTable: "College",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecture_Professsor_ProfessorId",
                table: "Lecture",
                column: "ProfessorId",
                principalTable: "Professsor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Professsor_College_CollegeId",
                table: "Professsor",
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
    }
}
