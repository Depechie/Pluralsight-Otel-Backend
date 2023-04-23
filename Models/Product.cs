using System;

namespace Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? ImageURL { get; set; }
        public string? HoverImageURL { get; set; }
        public string? ThumbnailImageURL { get; set; }
        public decimal? Price { get; set; }
        public int Rating { get; set; } = 0;

        public Product(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}