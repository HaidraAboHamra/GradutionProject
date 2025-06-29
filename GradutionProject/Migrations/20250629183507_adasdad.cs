using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradutionProject.Migrations
{
    /// <inheritdoc />
    public partial class adasdad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Admins_AdminId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ProfessorId",
                table: "Courses");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId",
                table: "News",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProfessorId",
                table: "News",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GradeOfStudents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DegreeOfStudiom = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DegreeOfLabs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CoursesId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeOfStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeOfStudents_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GradeOfStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$9pZSxyMXmpfXIX9oGamqXONBpqXuOVHpqB9tDTbuwao786IZvpa1q");

            migrationBuilder.CreateIndex(
                name: "IX_News_ProfessorId",
                table: "News",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ProfessorId",
                table: "Courses",
                column: "ProfessorId",
                unique: true,
                filter: "[ProfessorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GradeOfStudents_CoursesId",
                table: "GradeOfStudents",
                column: "CoursesId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeOfStudents_StudentId",
                table: "GradeOfStudents",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Admins_AdminId",
                table: "News",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Professors_ProfessorId",
                table: "News",
                column: "ProfessorId",
                principalTable: "Professors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Admins_AdminId",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Professors_ProfessorId",
                table: "News");

            migrationBuilder.DropTable(
                name: "GradeOfStudents");

            migrationBuilder.DropIndex(
                name: "IX_News_ProfessorId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ProfessorId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ProfessorId",
                table: "News");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEORnOyHZWpGTFS206rXM8pdrBz/Y6pJVOVO8gnGRg6hlLw0VLtacH0ZIGx5Rk9/a0A==");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ProfessorId",
                table: "Courses",
                column: "ProfessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Admins_AdminId",
                table: "News",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
