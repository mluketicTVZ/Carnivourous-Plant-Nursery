namespace Carnivorous_Plant_Nursery.Models
{
    public abstract class Article
    {
        public int Id { get; set; }
        public string? SKU { get; set; }
        public string? ListingTitle { get; set; }
        public decimal? Price { get; set; }
        public bool IsAvailableInWebshop { get; set; }
        public string? Description { get; set; }

        protected Article() {}
    }
}
