using Microsoft.AspNetCore.Mvc;
using CustomerHub.Data;
using CustomerHub.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using CustomerHub.Services;

namespace CustomerHub.Controllers
{
    public class CustomerController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly CustomerService _customerService;

        public CustomerController(DatabaseHelper db, CustomerService customerService)
        {
            _db = db;
            _customerService = customerService;
        }

        public IActionResult AdminCustomers()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            List<AdminCustomer> model = _customerService.GetAdminCustomers(userId.Value);
            return View(model);
        }


        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Customer> customers = _db.GetCustomersForUser(userId.Value);
            return View(customers);
        }

        
        [HttpPost]
        public IActionResult Create(string name, string address, string phone)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            int newCustomerId = _db.CreateCustomerForUser(userId.Value, name);

            if (!string.IsNullOrWhiteSpace(address))
            {
                _db.CreateAddress(newCustomerId, address);
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                _db.CreatePhone(newCustomerId, phone);
            }

            return RedirectToAction("Index");
        }

        
        [HttpPost]
        public IActionResult UpdateCustomer(int customerId, string newName)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            _db.UpdateCustomerName(customerId, newName);
            return Ok();
        }

        
        [HttpPost]
        public IActionResult DeleteCustomer(int customerId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            _db.DeleteCustomer(customerId);
            return Ok();
        }

        
        // Adres endpointleri

        [HttpGet]
        public IActionResult GetAddresses(int customerId)
        {
            // Optionally, you could verify that this customer belongs to the logged-in user.
            var addresses = _db.ListAddresses(customerId);
            return Json(addresses);
        }

        [HttpPost]
        public IActionResult CreateAddress(int customerId, string addressLine)
        {
            _db.CreateAddress(customerId, addressLine);
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateAddress(int addressId, string newValue)
        {
            _db.UpdateAddress(addressId, newValue);
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteAddress(int addressId)
        {
            _db.DeleteAddress(addressId);
            return Ok();
        }

        
        // Telefon endpointleri
        
        [HttpGet]
        public IActionResult GetPhones(int customerId)
        {
            var phones = _db.ListPhones(customerId);
            return Json(phones);
        }

        [HttpPost]
        public IActionResult CreatePhone(int customerId, string phoneNumber)
        {
            _db.CreatePhone(customerId, phoneNumber);
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdatePhone(int phoneId, string newValue)
        {
            _db.UpdatePhone(phoneId, newValue);
            return Ok();
        }

        [HttpPost]
        public IActionResult DeletePhone(int phoneId)
        {
            _db.DeletePhone(phoneId);
            return Ok();
        }
    }
}
