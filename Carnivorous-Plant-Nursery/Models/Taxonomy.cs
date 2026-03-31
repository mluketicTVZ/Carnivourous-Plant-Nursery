using System.Collections.Generic;

namespace Carnivorous_Plant_Nursery.Models
{
    public class Taxonomy
    {
        public int Id { get; set; }
        public string Genus { get; set; }     
        public string Species { get; set; }       
        public string Cultivar { get; set; }         
        public string CommonName { get; set; }      
        public int CareProfileId { get; set; }
        public CareProfile CareProfile { get; set; }

        public string FullName => string.IsNullOrEmpty(Cultivar) 
            ? $"{Genus} {Species}" 
            : $"{Genus} {Species} '{Cultivar}'";
            
        public List<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    }
}
