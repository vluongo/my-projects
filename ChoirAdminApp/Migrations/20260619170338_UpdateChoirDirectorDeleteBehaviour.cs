using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChoirAdminApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChoirDirectorDeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choir_Director_DirectorID",
                table: "Choir");

            migrationBuilder.AddForeignKey(
                name: "FK_Choir_Director_DirectorID",
                table: "Choir",
                column: "DirectorID",
                principalTable: "Director",
                principalColumn: "DirectorId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choir_Director_DirectorID",
                table: "Choir");

            migrationBuilder.AddForeignKey(
                name: "FK_Choir_Director_DirectorID",
                table: "Choir",
                column: "DirectorID",
                principalTable: "Director",
                principalColumn: "DirectorId");
        }
    }
}
