using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace REASite.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addres",
                columns: table => new
                {
                    HouseIndex = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    HouseNum = table.Column<string>(type: "text", nullable: false),
                    ApartmentID = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addres", x => x.HouseIndex);
                });

            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    apartment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AddresHouseIndex = table.Column<string>(type: "text", nullable: false),
                    Area = table.Column<int>(type: "integer", nullable: false),
                    RoomsCount = table.Column<byte>(type: "smallint", nullable: false),
                    OfferType = table.Column<string>(type: "text", nullable: false),
                    isFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    imgURL = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartments", x => x.apartment_id);
                    table.ForeignKey(
                        name: "FK_Apartments_Addres_AddresHouseIndex",
                        column: x => x.AddresHouseIndex,
                        principalTable: "Addres",
                        principalColumn: "HouseIndex",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_AddresHouseIndex",
                table: "Apartments",
                column: "AddresHouseIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.DropTable(
                name: "Addres");
        }
    }
}
