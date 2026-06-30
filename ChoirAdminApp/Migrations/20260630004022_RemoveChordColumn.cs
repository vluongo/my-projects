using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChoirAdminApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChordId",
                table: "Chorist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChordId",
                table: "Chorist",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
