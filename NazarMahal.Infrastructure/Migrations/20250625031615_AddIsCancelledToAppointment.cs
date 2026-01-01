using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NazarMahal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCancelledToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCancelled",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCancelled",
                table: "Appointments");
        }
    }
}
