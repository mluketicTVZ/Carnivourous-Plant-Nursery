using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carnivorous_Plant_Nursery.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Taxonomy",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CareProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Article",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Taxonomy");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CareProfile");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Article");
        }
    }
}
