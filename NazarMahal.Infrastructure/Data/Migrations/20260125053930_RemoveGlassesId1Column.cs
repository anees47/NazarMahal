using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NazarMahal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGlassesId1Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlassesAttachment_Glasses_GlassesId1",
                schema: "dbo",
                table: "GlassesAttachment");

            migrationBuilder.DropIndex(
                name: "IX_GlassesAttachment_GlassesId1",
                schema: "dbo",
                table: "GlassesAttachment");

            migrationBuilder.DropColumn(
                name: "GlassesId1",
                schema: "dbo",
                table: "GlassesAttachment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GlassesId1",
                schema: "dbo",
                table: "GlassesAttachment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlassesAttachment_GlassesId1",
                schema: "dbo",
                table: "GlassesAttachment",
                column: "GlassesId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GlassesAttachment_Glasses_GlassesId1",
                schema: "dbo",
                table: "GlassesAttachment",
                column: "GlassesId1",
                principalSchema: "dbo",
                principalTable: "Glasses",
                principalColumn: "Id");
        }
    }
}
