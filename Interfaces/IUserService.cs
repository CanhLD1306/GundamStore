using Microsoft.AspNetCore.Identity;


namespace GundamStore.Interfaces
{
    public interface IUserService
    {
        Task<SignInResult> AuthenticateUserAsync(string email, string password, bool rememberMe);
        Task SendRegisterEmailAsync(string email, string password);
        Task SendResetPasswordEmailAsync(string email);
        Task ResetPasswordAsync(string password);
        Task<string> GetUserRoleAsync(string email);
        Task<string> GetUserId();
        Task VerifyOTPToRegisterAsync(string OTP);
        Task VerifyOTPToResetPasswordAsync(string OTP);
        Task HandleGoogleLoginAsync(ExternalLoginInfo info);
    }
}


