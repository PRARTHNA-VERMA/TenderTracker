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
                    HttpContext.Session.SetString("user_type", Convert.ToString(result.Rows[0]["user_type"]));
                    HttpContext.Session.SetString("Name", Convert.ToString(result.Rows[0]["Name"]));
                    HttpContext.Session.SetString("Email", Convert.ToString(result.Rows[0]["Email"]));
                    HttpContext.Session.SetString("id", Convert.ToString(result.Rows[0]["id"]));
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
                    TempData["ErrorMessage"] = "Login Failed! Wrong Credentials";
                }

            }
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginForm");
        }
    }
}
