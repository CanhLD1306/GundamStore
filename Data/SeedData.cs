using GundamStore.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GundamStore.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Lấy UserManager và RoleManager từ DI container
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Tạo các role mặc định
            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Tạo tài khoản admin mặc định (nếu chưa có)
            var email = "Canhld1306@gmail.com";
            var password = "Admin@123"; // Mật khẩu phải tuân thủ các quy tắc bảo mật

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
                    // Gán role Admin cho tài khoản này
                    await userManager.AddToRoleAsync(user,"Admin");
                }
                else
                {
                    // Xử lý lỗi nếu tạo tài khoản không thành công
                    throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors)}");
                }
            }
        }
    }
}
