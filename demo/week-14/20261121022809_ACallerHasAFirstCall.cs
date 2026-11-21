using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab.Migrations
{
    /// <inheritdoc />
    public partial class ACallerHasAFirstCall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstCalled",
                table: "Callers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstCalled",
                table: "Callers");
        }
    }
}
