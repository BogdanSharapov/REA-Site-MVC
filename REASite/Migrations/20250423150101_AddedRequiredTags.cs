using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace REASite.Migrations
{
    /// <inheritdoc />
    public partial class AddedRequiredTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Addres_AddresHouseIndex",
                table: "Apartments");

            migrationBuilder.DeleteData(
                table: "Apartments",
                keyColumn: "apartment_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Apartments",
                keyColumn: "apartment_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Apartments",
                keyColumn: "apartment_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Apartments",
                keyColumn: "apartment_id",
                keyValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "AddresHouseIndex",
                table: "Apartments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Addres_AddresHouseIndex",
                table: "Apartments",
                column: "AddresHouseIndex",
                principalTable: "Addres",
                principalColumn: "HouseIndex",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Addres_AddresHouseIndex",
                table: "Apartments");

            migrationBuilder.AlterColumn<string>(
                name: "AddresHouseIndex",
                table: "Apartments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "Apartments",
                columns: new[] { "apartment_id", "AddresHouseIndex", "Area", "Description", "OfferType", "RoomsCount", "Title", "imgURL", "isFavorite" },
                values: new object[,]
                {
                    { 1, null, 0, "", "", (byte)0, "Apartment1", "", false },
                    { 2, null, 0, "", "", (byte)0, "Apartment2", "", false },
                    { 3, null, 0, "", "", (byte)0, "Apartment3", "", false },
                    { 4, null, 0, "", "", (byte)0, "Apartment4", "", false }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Addres_AddresHouseIndex",
                table: "Apartments",
                column: "AddresHouseIndex",
                principalTable: "Addres",
                principalColumn: "HouseIndex");
        }
    }
}
