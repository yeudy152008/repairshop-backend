using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairshopBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseDiscountAndIva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "PurchaseItems",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IvaRate",
                table: "PurchaseItems",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "IvaRate",
                table: "PurchaseItems");
        }
    }
}
