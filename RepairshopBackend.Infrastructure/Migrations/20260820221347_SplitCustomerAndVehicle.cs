using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairshopBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitCustomerAndVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Customers_CustomerId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Plate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VehicleBrand",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VehicleModel",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VehicleYear",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "WorkOrders",
                newName: "VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_CustomerId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_VehicleId");

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Plate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerId",
                table: "Vehicles",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Vehicles_VehicleId",
                table: "WorkOrders",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Vehicles_VehicleId",
                table: "WorkOrders");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "WorkOrders",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_VehicleId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_CustomerId");

            migrationBuilder.AddColumn<string>(
                name: "Plate",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleBrand",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleModel",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VehicleYear",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Customers_CustomerId",
                table: "WorkOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
