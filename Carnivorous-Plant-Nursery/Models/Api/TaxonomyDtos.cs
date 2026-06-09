using System.ComponentModel.DataAnnotations;

namespace Carnivorous_Plant_Nursery.Models.Api
{
    public class TaxonomyDto
    {
        public int Id { get; set; }
        public string? Genus { get; set; }
        public string? Species { get; set; }
        public string? Cultivar { get; set; }
        public string? CommonName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? CareProfileId { get; set; }
        public CareProfileSummaryDto? CareProfile { get; set; }
    }

    public class TaxonomySummaryDto
    {
        public int Id { get; set; }
        public string? CommonName { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class TaxonomyWriteDto
    {
        [MaxLength(200)]
        public string? Genus { get; set; }

        [MaxLength(200)]
        public string? Species { get; set; }

        [MaxLength(200)]
        public string? Cultivar { get; set; }

        [MaxLength(200)]
        public string? CommonName { get; set; }

        public int? CareProfileId { get; set; }
    }
}
