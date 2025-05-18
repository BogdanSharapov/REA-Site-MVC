using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REASite.Migrations
{
    /// <inheritdoc />
    public partial class ApartmentUpdatedAndUserTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Apartments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "floor",
                table: "Apartments",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "floor",
                table: "Apartments");
        }
    }
}
