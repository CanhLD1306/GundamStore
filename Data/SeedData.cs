using GundamStore.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GundamStore.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }


            var email = "Canhld1306@gmail.com";
            var password = "Admin@123";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new User
                {
                    UserName = email,
                    NormalizedUserName = "CANHLD1306@GMAIL.COM",
                    Email = email,
                    NormalizedEmail = "CANHLD1306@GMAIL.COM",
                    EmailConfirmed = true,
                    SecurityStamp = string.Empty
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {

                    await userManager.AddToRoleAsync(user,"Admin");
                }
                else
                {
                    throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors)}");
                }
            }
        }
    }
}
