using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NazarMahal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileDataToGlassesAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StoragePath",
                schema: "dbo",
                table: "GlassesAttachment",
                type: "NVARCHAR(500)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(500)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                schema: "dbo",
                table: "GlassesAttachment",
                type: "VARBINARY(MAX)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                schema: "dbo",
                table: "GlassesAttachment");

            migrationBuilder.AlterColumn<string>(
                name: "StoragePath",
                schema: "dbo",
                table: "GlassesAttachment",
                type: "NVARCHAR(500)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(500)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
