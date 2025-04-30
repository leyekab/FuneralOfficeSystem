using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuneralOfficeSystem.Migrations
{
    /// <inheritdoc />
    public partial class BurialPlaceAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "BurialPlaces",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "BurialPlaces",
                type: "TEXT",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "BurialPlaces");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "BurialPlaces");
        }
    }
}
