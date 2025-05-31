using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradutionProject.Migrations
{
    /// <inheritdoc />
    public partial class initsad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateImgPath",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "InvoicePath",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PersonalPhotoPath",
                table: "Students");

            migrationBuilder.AddColumn<byte[]>(
                name: "CertificateImg",
                table: "Students",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Invoice",
                table: "Students",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PersonalPhoto",
                table: "Students",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CertificateImg", "Invoice", "PersonalPhoto" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateImg",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Invoice",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PersonalPhoto",
                table: "Students");

            migrationBuilder.AddColumn<string>(
                name: "CertificateImgPath",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoicePath",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalPhotoPath",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CertificateImgPath", "InvoicePath", "PersonalPhotoPath" },
                values: new object[] { null, null, null });
        }
    }
}
