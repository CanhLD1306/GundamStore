using GundamStore.Models;
using Microsoft.AspNetCore.Identity;
using GundamStore.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using GundamStore.ViewModels;
using System.Security.Cryptography;
using System.Text;
using GundamStore.Common;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GundamStore.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        private readonly ILogger<UserService> _logger;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<UserService> logger,
            IEmailSenderService emailSenderService,
            IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSenderService = emailSenderService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SignInResult> AuthenticateUserAsync(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("This email is not registered.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                password,
                rememberMe,
                lockoutOnFailure: false
            );

            if (result == null)
            {
                throw new UnauthorizedAccessException("Authentication failed due to unknown error.");
            }

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Email or password is incorrect.");
            }

            if (result.IsLockedOut)
            {
                throw new UnauthorizedAccessException("This account is locked out. Please try again later or contact support.");
            }
            return result;
        }

        public async Task SendRegisterEmailAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                throw new ArgumentException("This email is already exists.");
            }

            var OTP = GenerateRandomOTP(8, includeSpecialChars: false);

            try
            {
                await _emailSenderService.SendEmailAsync(
                    email,
                    "Confirm Your Email",
                    $"Your verification OTP is: <strong>{OTP}</strong>. Please enter this OTP to complete your registration."
                );
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Failed to send verification email. Please try again later.");
            }

            SetUserSession(email, password, OTP);
        }

        public async Task VerifyOTPToRegisterAsync(string OTP)
        {
            var userSession = GetUserSession();

            if (userSession.OTP != OTP)
            {
                throw new ArgumentException("Invalid verification OTP.");
            }


            var user = new User
            {
                UserName = GetUsernameFromEmail(userSession.email!),
                Email = userSession.email,
                EmailConfirmed = true
            };

            var registerResult = await _userManager.CreateAsync(user, userSession.password);

            if (!registerResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to register user.");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to assign role to the user.");
            }
            _httpContextAccessor.HttpContext?.Session.Remove("UserSession");
        }

        public async Task SendResetPasswordEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new InvalidOperationException("This email does not exists.");
            }

            var OTP = GenerateRandomOTP(8, includeSpecialChars: false);

            try
            {
                await _emailSenderService.SendEmailAsync(
                    email,
                    "Confirm Your Email",
                    $"Your verification OTP is: <strong>{OTP}</strong>. Please enter this OTP to complete your registration."
                );
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Failed to send verification email. Please try again later.");
            }

            SetUserSession(email, "", OTP);

        }
        public Task VerifyOTPToResetPasswordAsync(string OTP)
        {
            var userSession = GetUserSession();

            if (userSession.OTP != OTP)
            {
                throw new ArgumentException("Invalid verification OTP.");
            }

            return Task.CompletedTask;
        }

        public async Task ResetPasswordAsync(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            var userSession = GetUserSession();
            var user = await _userManager.FindByEmailAsync(userSession.email);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to reset password.");
            }

            _httpContextAccessor.HttpContext?.Session.Remove("UserSession");
        }

        public async Task<GoogleLoginResult> HandleGoogleLoginAsync(ExternalLoginInfo info)
        {
            if (info == null)
            {
                return GoogleLoginResult.Canceled;
            }

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return GoogleLoginResult.AlreadyRegisteredWithGoogle;
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (addLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return GoogleLoginResult.AddedGoogleLoginToExistingAccount;
                }

                throw new InvalidOperationException("Error adding external login.");
            }

            user = new User
            {
                UserName = GetUsernameFromEmail(email),
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to create user.");
            }

            var addLoginResultNew = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResultNew.Succeeded)
            {
                throw new InvalidOperationException("Failed to add external login.");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to assign role.");
            }

            SetUserSession(email, "", "");
            return GoogleLoginResult.NewAccountCreated;
        }

        public async Task<string> GetUserRoleAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(Roles.Admin))
            {
                return Roles.Admin;
            }

            return Roles.Customer;
        }

        public async Task<string> GetUserId()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("HttpContext is null. This method must be called within an HTTP request.");
            }

            var claimsPrincipal = _httpContextAccessor.HttpContext?.User;

            if (claimsPrincipal == null)
            {
                throw new UnauthorizedAccessException("User not logged in.");
            }

            var user = await _userManager.GetUserAsync(claimsPrincipal);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            return user.Id;
        }

        private string GenerateRandomOTP(int length, bool includeSpecialChars = false)
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

        private string GetUsernameFromEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            return email.Substring(0, atIndex);
        }

        private void SetUserSession(string email, string password, string OTP)
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session == null)
            {
                throw new InvalidOperationException("Session is not available.");
            }

            var userSession = new UserSession
            {
                email = email,
                password = password,
                OTP = OTP
            };

            var json = JsonConvert.SerializeObject(userSession);
            session.SetString("UserSession", json);
        }

        private UserSession GetUserSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session == null)
            {
                throw new InvalidOperationException("Session is not available.");
            }

            var json = session.GetString("UserSession");

            if (string.IsNullOrEmpty(json))
            {
                throw new KeyNotFoundException("UserSession not found in session.");
            }

            var userSession = JsonConvert.DeserializeObject<UserSession>(json);

            if (userSession == null)
            {
                throw new InvalidOperationException("UserSession deserialization failed.");
            }
            return userSession;
        }

    }
}