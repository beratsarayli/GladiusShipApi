using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GladiusShip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MarinaTableFixestwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "MarinaPrice",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "IsPassive",
                table: "MarinaPrice",
                newName: "Value");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "MarinaPrice",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "MarinaPrice");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "MarinaPrice",
                newName: "IsPassive");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "MarinaPrice",
                newName: "Name");
        }
    }
}
