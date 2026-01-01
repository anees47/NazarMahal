using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NazarMahal.Infrastructure.Migrations
{
    public partial class AddOrderItemsAndConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create OrderItems table
            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    GlassesId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Glasses_GlassesId",
                        column: x => x.GlassesId,
                        principalTable: "Glasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Update Orders table
            migrationBuilder.DropColumn(name: "GlassesId", table: "Orders");
            migrationBuilder.DropColumn(name: "Quantity", table: "Orders");
            migrationBuilder.RenameColumn(name: "Amount", table: "Orders", newName: "TotalAmount");
            
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Add unique constraint on Appointments
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentDate_AppointmentTime",
                table: "Appointments",
                columns: new[] { "AppointmentDate", "AppointmentTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_GlassesId",
                table: "OrderItems",
                column: "GlassesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OrderItems");
            
            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentDate_AppointmentTime",
                table: "Appointments");

            migrationBuilder.DropColumn(name: "FirstName", table: "Orders");
            migrationBuilder.DropColumn(name: "LastName", table: "Orders");
            migrationBuilder.DropColumn(name: "PaymentMethod", table: "Orders");
            
            migrationBuilder.RenameColumn(name: "TotalAmount", table: "Orders", newName: "Amount");
            
            migrationBuilder.AddColumn<int>(
                name: "GlassesId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

