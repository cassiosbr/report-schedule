using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportSchedule.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToReportSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ReportSchedules",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ReportSchedules");
        }
    }
}
