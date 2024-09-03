using Microsoft.AspNetCore.Identity;

namespace GundamStore.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public string? Image { get; set; }

    }
}
