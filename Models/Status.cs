using System.ComponentModel.DataAnnotations;

namespace GundamStore.Models
{
    public class Status
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Lable { get; set; }
    }
}