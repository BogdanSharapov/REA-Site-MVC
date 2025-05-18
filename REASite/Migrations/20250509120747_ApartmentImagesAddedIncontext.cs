using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REASite.Migrations
{
    /// <inheritdoc />
    public partial class ApartmentImagesAddedIncontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentImage_Apartments_ApartmentId",
                table: "ApartmentImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApartmentImage",
                table: "ApartmentImage");

            migrationBuilder.RenameTable(
                name: "ApartmentImage",
                newName: "ApartmentImages");

            migrationBuilder.RenameIndex(
                name: "IX_ApartmentImage_ApartmentId",
                table: "ApartmentImages",
                newName: "IX_ApartmentImages_ApartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApartmentImages",
                table: "ApartmentImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentImages_Apartments_ApartmentId",
                table: "ApartmentImages",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "apartment_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentImages_Apartments_ApartmentId",
                table: "ApartmentImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApartmentImages",
                table: "ApartmentImages");

            migrationBuilder.RenameTable(
                name: "ApartmentImages",
                newName: "ApartmentImage");

            migrationBuilder.RenameIndex(
                name: "IX_ApartmentImages_ApartmentId",
                table: "ApartmentImage",
                newName: "IX_ApartmentImage_ApartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApartmentImage",
                table: "ApartmentImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentImage_Apartments_ApartmentId",
                table: "ApartmentImage",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "apartment_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
