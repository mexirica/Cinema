using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUnassignedSeatsinScreeningSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UnassignedSeat",
                table: "SaleScreenings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EIDR",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_EIDR",
                table: "Movies",
                column: "EIDR",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_EIDR",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "UnassignedSeat",
                table: "SaleScreenings");

            migrationBuilder.DropColumn(
                name: "EIDR",
                table: "Movies");
        }
    }
}
