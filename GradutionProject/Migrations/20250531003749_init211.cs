using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradutionProject.Migrations
{
    /// <inheritdoc />
    public partial class init211 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "Email", "Name", "Password", "Phone" },
                values: new object[] { 1, "Admin@Admin.com", "Admin", "AQAAAAIAAYagAAAAEORnOyHZWpGTFS206rXM8pdrBz/Y6pJVOVO8gnGRg6hlLw0VLtacH0ZIGx5Rk9/a0A==", "999" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Birth", "CertificateDate", "CertificateImg", "College", "Email", "Gender", "Invoice", "Name", "NationalId", "PasswordHash", "PersonalPhoto", "PhoneNumber" },
                values: new object[] { 1, null, null, null, null, "Admin@Admin.com", null, null, "Admin", null, null, null, null });
        }
    }
}
