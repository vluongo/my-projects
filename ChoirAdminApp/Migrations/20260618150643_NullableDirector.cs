using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChoirAdminApp.Migrations
{
    /// <inheritdoc />
    public partial class NullableDirector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choir_Director_DirectorID",
                table: "Choir");

            migrationBuilder.AlterColumn<Guid>(
                name: "DirectorID",
                table: "Choir",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Choir_Director_DirectorID",
                table: "Choir",
                column: "DirectorID",
                principalTable: "Director",
                principalColumn: "DirectorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choir_Director_DirectorID",
                table: "Choir");

            migrationBuilder.AlterColumn<Guid>(
                name: "DirectorID",
                table: "Choir",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Choir_Director_DirectorID",
                table: "Choir",
                column: "DirectorID",
                principalTable: "Director",
                principalColumn: "DirectorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
