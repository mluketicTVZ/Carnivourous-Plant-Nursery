using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carnivorous_Plant_Nursery.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedBatchAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PlantId",
                table: "Attachment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SeedBatchId",
                table: "Attachment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_SeedBatchId",
                table: "Attachment",
                column: "SeedBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Article_SeedBatchId",
                table: "Attachment",
                column: "SeedBatchId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Article_SeedBatchId",
                table: "Attachment");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_SeedBatchId",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "SeedBatchId",
                table: "Attachment");

            migrationBuilder.AlterColumn<int>(
                name: "PlantId",
                table: "Attachment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
