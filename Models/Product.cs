﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Product
    {

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        public long CategoryId { get; set; }

        [Required]
        public long ScaleId { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Brand { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public int Discount { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int Sold { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public string? CreatedBy { get; set; }

        [Required]
        public string? UpdatedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public Category? Category { get; set; }
        public Scale? Scale { get; set; }

    }
}
