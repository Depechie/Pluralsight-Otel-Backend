using System;

namespace Models
{
    public class Concert
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageURL { get; set; }
        public string? HoverImageURL { get; set; }
        public string? ThumbnailImageURL { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Headliner { get; set; }
        public List<string> Artists { get; set; } = new List<string>();        
        public decimal? Price { get; set; }

        public Concert(string id, string name, string description, string headliner)
        {
            Id = id;
            Name = name;
            Description = description;
            Headliner = headliner;
        }
    }
}