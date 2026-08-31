using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairshopBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerExemptionLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExemptionLetterNumber",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExemptionLetterNumber",
                table: "Customers");
        }
    }
}
