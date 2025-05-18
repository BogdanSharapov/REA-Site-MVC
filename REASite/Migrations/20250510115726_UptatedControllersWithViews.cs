using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REASite.Migrations
{
    /// <inheritdoc />
    public partial class UptatedControllersWithViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "floor",
                table: "Apartments",
                newName: "Floor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Floor",
                table: "Apartments",
                newName: "floor");
        }
    }
}
