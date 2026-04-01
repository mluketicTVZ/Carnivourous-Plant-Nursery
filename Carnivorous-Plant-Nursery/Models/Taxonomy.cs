using System.Collections.Generic;

namespace Carnivorous_Plant_Nursery.Models
{
    public class Taxonomy
    {
        public int Id { get; set; }
        public string? Genus { get; set; }
        public string? Species { get; set; }
        public string? Cultivar { get; set; }
        public string? CommonName { get; set; }
        public int? CareProfileId { get; set; }
        public CareProfile? CareProfile { get; set; }

        public string FullName 
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(Genus)) parts.Add(Genus);
                if (!string.IsNullOrEmpty(Species)) parts.Add(Species);
                if (!string.IsNullOrEmpty(Cultivar)) parts.Add($"'{Cultivar}'");
                
                return parts.Count > 0 ? string.Join(" ", parts) : DisplayConstant.UnknownFullName;
            }
        }

        public List<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    }
}
