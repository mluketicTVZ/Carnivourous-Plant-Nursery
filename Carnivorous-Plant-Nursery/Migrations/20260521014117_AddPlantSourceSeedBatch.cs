using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carnivorous_Plant_Nursery.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantSourceSeedBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceSeedBatchId",
                table: "Article",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Article_SourceSeedBatchId",
                table: "Article",
                column: "SourceSeedBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Article_SourceSeedBatchId",
                table: "Article",
                column: "SourceSeedBatchId",
                principalTable: "Article",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_Article_SourceSeedBatchId",
                table: "Article");

            migrationBuilder.DropIndex(
                name: "IX_Article_SourceSeedBatchId",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "SourceSeedBatchId",
                table: "Article");
        }
    }
}
