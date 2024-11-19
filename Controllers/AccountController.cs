
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GundamStore.Models;
using GundamStore.ViewModels;
using GundamStore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;


namespace GundamStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSenderService _emailSenderService;

        private readonly IUserService _userService;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<AccountController> logger,
            IEmailSenderService emailSenderService,
            IUserService userService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSenderService = emailSenderService;
            _userService = userService;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new Result { Success = false, Message = "Invalid login details !" });
                }

                await _userService.AuthenticateUserAsync(model.Email, model.Password!, model.RememberMe);
                var role = await _userService.GetUserRoleAsync(model.Email);

                if (role == "Admin")
                {
                    return Json(new Result
                    {
                        Success = true,
                        Message = "Login successfully !",
                        RedirectUrl = Url.Action("Dashboard", "Admin")
                    });
                }

                return Json(new Result
                {
                    Success = true,
                    Message = "Login successfully !",
                    RedirectUrl = ReturnUrl ?? Url.Action("Index", "Home")
                });
            }
            catch (Exception ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new Result { Success = false, Message = "Invalid register details !" });
                }

                await _userService.SendRegisterEmailAsync(model.Email, model.Password);
                return Json(new Result
                {
                    Success = true,
                    Message = "Verification OTP sent. Please check your email.",
                    RedirectUrl = Url.Action("VerifyOTP", "Account", new { type = "register" })
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An unexpected error occurred. Please try again." });
            }
        }

        public IActionResult VerifyOTP(string type)
        {
            ViewBag.type = type;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOTPToRegister(string OTP)
        {
            try
            {
                await _userService.VerifyOTPToRegisterAsync(OTP);
                return Json(new Result
                {
                    Success = true,
                    Message = "Register successfully !",
                    RedirectUrl = Url.Action("Login", "Account")
                });
            }
            catch (Exception ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
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
                return Json(new Result
                {
                    Success = false,
                    Message = "Error loading external login information.",
                    RedirectUrl = Url.Action("Login", "Account")
                });
            }

            try
            {
                await _userService.HandleGoogleLoginAsync(info);
                return Json(new Result
                {
                    Success = true,
                    Message = "Login successfully !",
                    RedirectUrl = Url.Action("ResetPassword", "Account")
                });
            }
            catch (Exception ex)
            {
                return Json(new Result
                {
                    Success = false,
                    Message = ex.Message,
                    RedirectUrl = Url.Action("Login", "Account")
                });
            }

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
        public async Task<IActionResult> SendVerificationEmail(string email)
        {
            try
            {
                await _userService.SendResetPasswordEmailAsync(email);
                return Json(new Result
                {
                    Success = true,
                    Message = "Verification OTP sent. Please check your email.",
                    RedirectUrl = Url.Action("VerifyOTP", "Account", new { type = "reset" })
                });
            }
            catch (Exception ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult VerifyOTPToResetPassword(string OTP)
        {
            try
            {
                _userService.VerifyOTPToResetPasswordAsync(OTP);
                return Json(new Result
                {
                    Success = true,
                    Message = "Reset password successfully !",
                    RedirectUrl = Url.Action("ResetPassword", "Account")
                });
            }
            catch (Exception ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
        }

        public IActionResult ResetPassword()
        {
            return View(new NewPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(NewPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new Result { Success = false, Message = "Invalid input data" });
                }

                await _userService.ResetPasswordAsync(model.Password);

                return Json(new Result
                {
                    Success = true,
                    Message = "Reset password successfully !",
                    RedirectUrl = Url.Action("Login", "Account")
                });
            }
            catch (Exception ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
        }

    }
}