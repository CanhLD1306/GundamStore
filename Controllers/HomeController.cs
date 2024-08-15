using GundamStore.Interfaces;
using GundamStore.Models;
using GundamStore.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;


namespace GundamStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly IEmailSenderService _emailSenderService;
        public HomeController(ILogger<HomeController> logger, IFirebaseStorageService firebaseStorageService, IEmailSenderService emailSenderService)
        {
            _logger = logger;
            _firebaseStorageService = firebaseStorageService;
            _emailSenderService = emailSenderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> TestFirebaseUpload(IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    var fileUrl = await _firebaseStorageService.UploadFileAsync(file, "TestUploads");
                    return Ok(new { Message = "File uploaded successfully.", Url = fileUrl });
                }
                catch (Exception ex)
                {
                    return BadRequest($"Firebase upload failed: {ex.Message}");
                }
            }

            return BadRequest("No file uploaded.");
        }

        public async Task<IActionResult> TestEmailSender()
        {
            var email = new EmailMessages
            {
                toEmail = "lienminhco1023@gmail.com",
                subject = "Test Email",
                body = "This is a test email from GundamStore."
            };

            try
            {
                await _emailSenderService.SendEmailAsync(email.toEmail, email.subject, email.body);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Email send failed: {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
