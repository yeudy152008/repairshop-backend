using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairshopBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIvaRateAndExemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Exonerado",
                table: "InvoiceItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "IvaRate",
                table: "InvoiceItems",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IvaRate",
                table: "InventoryItems",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exonerado",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "IvaRate",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "IvaRate",
                table: "InventoryItems");
        }
    }
}
