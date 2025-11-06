using System.Data;
using Microsoft.AspNetCore.Mvc;
using TenderTracker.Models;
using TenderTracker.Repository;

namespace TenderTracker.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly LoginRepository _LoginRepo;
        public LoginController(ILogger<LoginController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _LoginRepo = new LoginRepository(configuration);
        }
        public IActionResult LoginForm()
        {
            return View();
        }

        [HttpPost]

        public IActionResult LoginForm(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                DataTable result = _LoginRepo.MatchUser(model);
                if (result.Rows.Count > 0)
                {
                    var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");

                    if (model.CaptchaCode != sessionCaptcha)
                    {
                        ViewData["CaptchaError"] = "Invalid CAPTCHA code.";
                        return View(model);
                    }

                    HttpContext.Session.SetString("user_type", Convert.ToString(result.Rows[0]["user_type"]));
                    HttpContext.Session.SetString("Name", Convert.ToString(result.Rows[0]["Name"]));
                    HttpContext.Session.SetString("Email", Convert.ToString(result.Rows[0]["Email"]));
                    HttpContext.Session.SetString("id", Convert.ToString(result.Rows[0]["id"]));
                    //TempData["SuccessMessage"] = "Welcome to Dashboard";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
                    TempData["ErrorMessage"] = "Login Failed! Wrong Credentials";
                }

            }
            return View();
        }

        public IActionResult GenerateCaptcha()
        {
            var captchaText = GenerateRandomCode();
            HttpContext.Session.SetString("CaptchaCode", captchaText);

            using var bitmap = new System.Drawing.Bitmap(100, 30);
            using var g = System.Drawing.Graphics.FromImage(bitmap);
            g.Clear(System.Drawing.Color.White);
            g.DrawString(captchaText, new System.Drawing.Font("Arial", 16), System.Drawing.Brushes.Black, new System.Drawing.PointF(10, 5));

            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }

        private string GenerateRandomCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public IActionResult LogOut()
        {
            //clear session
            HttpContext.Session.Clear();

            // Clear TempData
            foreach (var key in TempData.Keys.ToList())
            {
                TempData.Remove(key);
            }
            return RedirectToAction("LoginForm");
        }
    }
}
