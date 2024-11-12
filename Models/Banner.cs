using System;
using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Banner
    {
        public long Id { get; set; }

        [Required]
        [Url]
        public string? FileImage { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

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
    }
}