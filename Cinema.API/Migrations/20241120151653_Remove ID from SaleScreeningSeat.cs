using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIDfromSaleScreeningSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "SaleScreeningSeats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SaleScreeningSeats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
