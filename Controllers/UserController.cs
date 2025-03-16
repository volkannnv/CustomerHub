using Microsoft.AspNetCore.Mvc;
using CustomerHub.Data;
using CustomerHub.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerHub.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseHelper _db;

        public UserController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Settings()
        {
            
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            
            User currentUser = _db.GetUserById(userId.Value);
            if (currentUser == null)
            {
                
                return RedirectToAction("Logout", "Account");
            }

            return View(currentUser);
        }

        [HttpPost]
        public IActionResult Update(string newUsername, string newPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            
            _db.UpdateUser(userId.Value, newUsername, newPassword);

            
            HttpContext.Session.SetString("Username", newUsername);

            
            return RedirectToAction("Settings");
        }
    }
}
