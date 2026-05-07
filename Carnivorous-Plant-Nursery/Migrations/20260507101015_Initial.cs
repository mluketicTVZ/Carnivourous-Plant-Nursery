using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carnivorous_Plant_Nursery.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CareProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CareProfileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequiredLight = table.Column<int>(type: "int", nullable: true),
                    MinTemperature = table.Column<int>(type: "int", nullable: true),
                    MaxTemperature = table.Column<int>(type: "int", nullable: true),
                    TemperatureDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiresWinterDormancy = table.Column<bool>(type: "bit", nullable: true),
                    SoilMix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequiredHumidity = table.Column<int>(type: "int", nullable: true),
                    CareDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Taxonomy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Species = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Cultivar = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CommonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CareProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxonomy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Taxonomy_CareProfile_CareProfileId",
                        column: x => x.CareProfileId,
                        principalTable: "CareProfile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ListingTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsAvailableInWebshop = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    TaxonomyId = table.Column<int>(type: "int", nullable: true),
                    LineageId = table.Column<int>(type: "int", nullable: true),
                    DateAcquired = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LocationInNursery = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentStage = table.Column<int>(type: "int", nullable: true),
                    PotDiameterCm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PotHeightCm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastRepottingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDormancyDateStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDormancyDateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedAgeAtAcquiryYears = table.Column<int>(type: "int", nullable: true),
                    HealthStatus = table.Column<int>(type: "int", nullable: true),
                    HealthDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SeedCount = table.Column<int>(type: "int", nullable: true),
                    HarvestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedViabilityMonths = table.Column<int>(type: "int", nullable: true),
                    RequiresStratification = table.Column<bool>(type: "bit", nullable: true),
                    EstimatedGerminationRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                    table.CheckConstraint("CK_Article_Price_NonNegative", "[Price] >= 0");
                    table.CheckConstraint("CK_Plant_EstimatedAgeAtAcquiryYears_NonNegative", "[EstimatedAgeAtAcquiryYears] >= 0");
                    table.CheckConstraint("CK_Plant_PotDiameterCm_NonNegative", "[PotDiameterCm] >= 0");
                    table.CheckConstraint("CK_Plant_PotHeightCm_NonNegative", "[PotHeightCm] >= 0");
                    table.CheckConstraint("CK_SeedBatch_EstimatedGerminationRate_Range", "[EstimatedGerminationRate] >= 0 AND [EstimatedGerminationRate] <= 1");
                    table.CheckConstraint("CK_SeedBatch_SeedCount_NonNegative", "[SeedCount] >= 0");
                    table.ForeignKey(
                        name: "FK_Article_Taxonomy_TaxonomyId",
                        column: x => x.TaxonomyId,
                        principalTable: "Taxonomy",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Lineage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MotherId = table.Column<int>(type: "int", nullable: true),
                    FatherId = table.Column<int>(type: "int", nullable: true),
                    Generation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsClone = table.Column<bool>(type: "bit", nullable: true),
                    GeneticsDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lineage_Article_FatherId",
                        column: x => x.FatherId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lineage_Article_MotherId",
                        column: x => x.MotherId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_LineageId",
                table: "Article",
                column: "LineageId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_TaxonomyId",
                table: "Article",
                column: "TaxonomyId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineage_FatherId",
                table: "Lineage",
                column: "FatherId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineage_MotherId",
                table: "Lineage",
                column: "MotherId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxonomy_CareProfileId",
                table: "Taxonomy",
                column: "CareProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Lineage_LineageId",
                table: "Article",
                column: "LineageId",
                principalTable: "Lineage",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_Lineage_LineageId",
                table: "Article");

            migrationBuilder.DropTable(
                name: "Lineage");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Taxonomy");

            migrationBuilder.DropTable(
                name: "CareProfile");
        }
    }
}
