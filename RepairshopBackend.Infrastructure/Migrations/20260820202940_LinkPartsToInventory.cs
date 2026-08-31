using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairshopBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkPartsToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InventoryItemId",
                table: "WorkOrderParts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderParts_InventoryItemId",
                table: "WorkOrderParts",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderParts_InventoryItems_InventoryItemId",
                table: "WorkOrderParts",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderParts_InventoryItems_InventoryItemId",
                table: "WorkOrderParts");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderParts_InventoryItemId",
                table: "WorkOrderParts");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                table: "WorkOrderParts");
        }
    }
}
