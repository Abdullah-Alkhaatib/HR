using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class remove_age_add_birthdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Employees");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Employees",
                type: "int",
                nullable: true);
        }
    }
}
