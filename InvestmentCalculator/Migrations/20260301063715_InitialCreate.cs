using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentCalculator.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calculations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InitialAmount = table.Column<double>(type: "REAL", nullable: false),
                    MonthlyContribution = table.Column<double>(type: "REAL", nullable: false),
                    InterestRate = table.Column<double>(type: "REAL", nullable: false),
                    Years = table.Column<int>(type: "INTEGER", nullable: false),
                    FutureValue = table.Column<double>(type: "REAL", nullable: false),
                    TotalInvested = table.Column<double>(type: "REAL", nullable: false),
                    TotalInterest = table.Column<double>(type: "REAL", nullable: false),
                    CalculationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calculations");
        }
    }
}
