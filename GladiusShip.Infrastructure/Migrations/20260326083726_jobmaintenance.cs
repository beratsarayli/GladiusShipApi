using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GladiusShip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class jobmaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaintenanceJob",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonalRef = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FormType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsiblePersonal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJob", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobAction",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MasterComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextActionHour = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobAction", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobCost",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LaborCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoiceFile = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobCost", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobItem",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarrantyFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPassive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobItem", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobPhoto",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobPhoto", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobRisk",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EngineHourBefore = table.Column<int>(type: "int", nullable: true),
                    EngineHourAfter = table.Column<int>(type: "int", nullable: true),
                    HasWaste = table.Column<bool>(type: "bit", nullable: false),
                    WasteDetail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobRisk", x => x.Ref);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceJob");

            migrationBuilder.DropTable(
                name: "MaintenanceJobAction");

            migrationBuilder.DropTable(
                name: "MaintenanceJobCost");

            migrationBuilder.DropTable(
                name: "MaintenanceJobItem");

            migrationBuilder.DropTable(
                name: "MaintenanceJobPhoto");

            migrationBuilder.DropTable(
                name: "MaintenanceJobRisk");
        }
    }
}
