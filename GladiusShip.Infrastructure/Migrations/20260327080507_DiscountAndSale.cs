using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GladiusShip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DiscountAndSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsuranceDiscount",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsuranceRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceDiscount", x => x.Ref);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceCostDocument",
                columns: table => new
                {
                    Ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceRef = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPassive = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceCostDocument", x => x.Ref);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsuranceDiscount");

            migrationBuilder.DropTable(
                name: "MaintenanceCostDocument");
        }
    }
}
