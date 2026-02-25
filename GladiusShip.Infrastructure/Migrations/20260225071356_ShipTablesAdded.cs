using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GladiusShip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShipTablesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ship",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyRef = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HullId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImoNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ship", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "ShipDocument",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermitValidity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    İnsuranceExpire = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RadioCallSign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MMSINumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CEDocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipDocument", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "ShipMachine",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineBrand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineSerial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    EngineClock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipMachine", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "ShipPhoto",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipPhoto", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "ShipRegistration",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationSize = table.Column<int>(type: "int", nullable: false),
                    RegistrationWidth = table.Column<int>(type: "int", nullable: false),
                    GrossTonilato = table.Column<int>(type: "int", nullable: false),
                    ShipCreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipRegistration", x => x.Ref);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ship");

            migrationBuilder.DropTable(
                name: "ShipDocument");

            migrationBuilder.DropTable(
                name: "ShipMachine");

            migrationBuilder.DropTable(
                name: "ShipPhoto");

            migrationBuilder.DropTable(
                name: "ShipRegistration");
        }
    }
}
