
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GundamStore.Interfaces;
using GundamStore.Models;
using GundamStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;


namespace GundamStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSenderService _emailSenderService;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<AccountController> logger,
            IEmailSenderService emailSenderService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSenderService = emailSenderService;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                if (model.Email != null)
                {
                    return await HandleSuccessfulLogin(model.Email);
                }
                else
                {
                    return BadRequest("Email is required.");
                }
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = Url.Content("~/"), model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
        
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {       
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password))
            {
                // Tạm thời lưu thông tin người dùng vào session
                // Gửi email xác nhận với mã xác nhận
                var code = GenerateRandomCode(8, includeSpecialChars: false);
                HttpContext.Session.SetString("Email", model.Email);
                HttpContext.Session.SetString("Password", model.Password);
                HttpContext.Session.SetString("VerifyCode", code);

                await _emailSenderService.SendEmailAsync(
                    model.Email,
                    "Confirm Your Email",
                    $"Your verification code is: <strong>{code}</strong>. Please enter this code to complete your registration."
                );

                return RedirectToAction("VerifyCode", new { type = "register"});
            }

            return View(model);
        }

        public IActionResult GoogleSignUp(string? returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        public async Task<IActionResult> GoogleResponse(string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return await HandleSuccessfulLogin(user.Email);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (addLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return await HandleSuccessfulLogin(user.Email);
                }
                ModelState.AddModelError(string.Empty, "Error adding external login.");
                foreach (var error in addLoginResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction("Login");
            }

            user = new User
            {
                UserName = email,
                Email = email,
            };

            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (addLoginResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                    if (roleResult.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (updateResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return RedirectToAction("ResetPassword");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to update user.");
                            foreach (var error in updateResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to assign role.");
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add external login.");
                    foreach (var error in addLoginResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Failed to create user.");
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("Register");
        }

        public IActionResult VerifyCode(string type)
        {
            ViewBag.type = type;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyCodeToRegister(string code)
        {
            // Lấy thông tin từ session
            var email = HttpContext.Session.GetString("Email");
            var password = HttpContext.Session.GetString("Password");
            var verifyCode = HttpContext.Session.GetString("VerifyCode");

            if (email == null || password == null || verifyCode == null)
            {
                ModelState.AddModelError("", "Session expired or invalid.");
                return RedirectToAction("VerifyCode", new { type = "register" });
            }

            if (verifyCode == code)
            {
                // Tạo mới người dùng với email và password
                var newUser = new User { UserName = email, Email = email };
                var result = await _userManager.CreateAsync(newUser, password);

                if (result.Succeeded)
                {
                    // Cập nhật trạng thái email đã xác thực
                    var user = await _userManager.FindByEmailAsync(email);

                    var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to assign role to the user.");
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return RedirectToAction("VerifyCode", new {type = "register"});
                    }

                    user.EmailConfirmed = true;
                    var updateUser = await _userManager.UpdateAsync(user);
                    if(updateUser.Succeeded)
                    {
                        // Xóa thông tin từ session sau khi hoàn tất
                        HttpContext.Session.Clear();
                        
                        // Đăng nhập người dùng và chuyển hướng đến trang chính
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Thêm lỗi nếu không thể tạo người dùng
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    // Thêm lỗi nếu không thể tạo người dùng
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid verification code.");
            }

            return RedirectToAction("Verifycode", new { type = "register" });
        }
        
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                await _signInManager.SignOutAsync();

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Login", "Account");
                }
                else if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult SendEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(string email)
        {
            var code = GenerateRandomCode(8, includeSpecialChars: false);
            HttpContext.Session.SetString("VerifyCode", code);
            HttpContext.Session.SetString("Email", email);
            await _emailSenderService.SendEmailAsync(
                email,
                "Reset Password",
                $"Your reset password code is: <strong>{code}</strong>. Please enter this code to reset your password."
            );
            return RedirectToAction("Verifycode", new { type = "reset"});
        }

        [HttpPost]
        public IActionResult VerifyCodeToResetPassword(string code)
        {
            var verifyCode = HttpContext.Session.GetString("VerifyCode");

            if (verifyCode == code)
            {
                return RedirectToAction("ResetPassword");
            }

            ModelState.AddModelError("", "Invalid verification code.");
            return RedirectToAction("VerifyCode", new { type = "reset" });
        }

        public IActionResult ResetPassword()
        {
            var model = new NewPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(NewPasswordViewModel model)
        {
            if(ModelState.IsValid && !string.IsNullOrEmpty(model.Password))
            {
                if(User?.Identity?.IsAuthenticated == true)
                {
                    return await SetPasswordForGoogleSignUp(model.Password);
                }

                var email = HttpContext.Session.GetString("Email");
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return RedirectToAction("ResetPassword");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        private async Task<IActionResult> HandleSuccessfulLogin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                _logger.LogInformation("Admin logged in.");
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            if (roles.Contains("Customer"))
            {
                _logger.LogInformation("Customer logged in.");
                return RedirectToAction("Index", "Home");
            }

            // Xử lý cho các vai trò khác nếu cần
            return RedirectToAction("Index", "Home");
        }

        private string GenerateRandomCode(int length, bool includeSpecialChars = false)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            string allChars = upperChars + lowerChars + numbers;
            if (includeSpecialChars)
            {
                allChars += specialChars;
            }

            using (var rng = new RSACryptoServiceProvider())
            {
                var result = new StringBuilder(length);
                var buffer = new byte[8];

                for (int i = 0; i < length; i++)
                {
                    RandomNumberGenerator.Fill(buffer);
                    ulong randomNumber = BitConverter.ToUInt64(buffer, 0);
                    result.Append(allChars[(int)(randomNumber % (uint)allChars.Length)]);
                }

                return result.ToString();
            }
        }
    
        private async Task<IActionResult> SetPasswordForGoogleSignUp(string password)
        {
            var email = HttpContext.User.Identity?.Name;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("Index", "Home");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
   }
}