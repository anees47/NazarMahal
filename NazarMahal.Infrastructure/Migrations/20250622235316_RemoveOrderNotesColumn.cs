using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NazarMahal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderNotesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNotes",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderNotes",
                table: "Orders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
