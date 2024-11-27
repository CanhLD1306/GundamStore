using System.ComponentModel.DataAnnotations;

namespace GundamStore.Dto
{
    public class ProductWithImageDto
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public long CategoryId { get; set; }

        public long ScaleId { get; set; }

        public string? Brand { get; set; }

        public decimal Price { get; set; }

        public int Discount { get; set; }

        public int StockQuantity { get; set; }

        public int Sold { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Url]
        public string? ImageDefault { get; set; }

    }
}