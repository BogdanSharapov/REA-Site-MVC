using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REASite.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedApartmentImagesforLiteDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "ApartmentImages",
                newName: "ImageID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageID",
                table: "ApartmentImages",
                newName: "ImageURL");
        }
    }
}
