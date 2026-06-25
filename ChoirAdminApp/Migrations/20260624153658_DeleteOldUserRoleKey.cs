using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChoirAdminApp.Migrations
{
    /// <inheritdoc />
    public partial class DeleteOldUserRoleKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRoleId",
                table: "UserRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserRoleId",
                table: "UserRole",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
