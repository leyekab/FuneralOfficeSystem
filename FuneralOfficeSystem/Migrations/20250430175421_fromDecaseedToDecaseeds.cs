using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuneralOfficeSystem.Migrations
{
    /// <inheritdoc />
    public partial class fromDecaseedToDecaseeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Funerals_Deceased_DeceasedId",
                table: "Funerals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deceased",
                table: "Deceased");

            migrationBuilder.RenameTable(
                name: "Deceased",
                newName: "Deceaseds");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deceaseds",
                table: "Deceaseds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Funerals_Deceaseds_DeceasedId",
                table: "Funerals",
                column: "DeceasedId",
                principalTable: "Deceaseds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Funerals_Deceaseds_DeceasedId",
                table: "Funerals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deceaseds",
                table: "Deceaseds");

            migrationBuilder.RenameTable(
                name: "Deceaseds",
                newName: "Deceased");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deceased",
                table: "Deceased",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Funerals_Deceased_DeceasedId",
                table: "Funerals",
                column: "DeceasedId",
                principalTable: "Deceased",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
