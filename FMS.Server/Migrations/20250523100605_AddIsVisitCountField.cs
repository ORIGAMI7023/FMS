using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMS.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddIsVisitCountField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Doctor",
                table: "RevenueRecords",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "RevenueRecords",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisitCount",
                table: "RevenueRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisitCount",
                table: "RevenueRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Doctor",
                table: "RevenueRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "RevenueRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
