using System;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;

        [Required]
        public int Created_By { get; set; }

        [Required]
        public int Updated_By { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}