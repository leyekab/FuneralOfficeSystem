using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuneralOfficeSystem.Migrations
{
    /// <inheritdoc />
    public partial class WithIsEnable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsProductSupplier",
                table: "Suppliers",
                newName: "SupplierType");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "FuneralOffices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "FuneralOffices");

            migrationBuilder.RenameColumn(
                name: "SupplierType",
                table: "Suppliers",
                newName: "IsProductSupplier");
        }
    }
}
