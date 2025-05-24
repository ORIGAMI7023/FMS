using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMS.Server.Migrations
{
    /// <inheritdoc />
    public partial class RebuildRevenueStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "RevenueRecords");

            migrationBuilder.DropColumn(
                name: "Doctor",
                table: "RevenueRecords");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "RevenueRecords",
                newName: "Value");

            migrationBuilder.AddColumn<bool>(
                name: "IsExcludedFromSummary",
                table: "RevenueRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "RevenueRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExcludedFromSummary",
                table: "RevenueRecords");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "RevenueRecords");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "RevenueRecords",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "RevenueRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Doctor",
                table: "RevenueRecords",
                type: "TEXT",
                nullable: true);
        }
    }
}
