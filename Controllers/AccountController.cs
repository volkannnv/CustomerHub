using Microsoft.AspNetCore.Mvc;
using CustomerHub.Data;
using Microsoft.AspNetCore.Http;

namespace CustomerHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _db;

        public AccountController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(string email, string username, string password)
        {
            _db.CreateUser(email, username, password);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            bool valid = _db.ValidateUser(username, password);
            if (valid)
            {
                HttpContext.Session.SetString("Username", username);
                int userId = _db.GetUserIdByUsername(username);
                HttpContext.Session.SetInt32("UserId", userId);
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
