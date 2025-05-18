using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REASite.Migrations
{
    /// <inheritdoc />
    public partial class IndexForAddressAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Country_City_Street_HouseNum",
                table: "Addresses",
                columns: new[] { "Country", "City", "Street", "HouseNum" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_Country_City_Street_HouseNum",
                table: "Addresses");
        }
    }
}
