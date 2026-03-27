using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GladiusShip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MarinaTableFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Marina",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marina", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MarinaDetails",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarinaRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarinaDetails", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MarinaPermission",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarinaPermission", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MarinaPrice",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarinaRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarinaPrice", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MarinaRoad",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarinaRoad", x => x.Ref);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Marina");

            migrationBuilder.DropTable(
                name: "MarinaDetails");

            migrationBuilder.DropTable(
                name: "MarinaPermission");

            migrationBuilder.DropTable(
                name: "MarinaPrice");

            migrationBuilder.DropTable(
                name: "MarinaRoad");
        }
    }
}
