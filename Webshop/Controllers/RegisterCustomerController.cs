using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class RegisterCustomerController : Controller
    {
        private readonly LapWebshopContext _context;
        private readonly UserService _userService;


        public RegisterCustomerController(LapWebshopContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return  View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterCustomer customer, string password)
        {
            // Im Model überprüfen ob die Einschränkungen bei den Feldern eingehalten wurden (z.B. max Länge von Feldern)
            // Wenn nicht wird die View mit eingegebenen Daten und Fehlern angezeigt
            if (!ModelState.IsValid)
            {
                return View(customer);
            }
            else
            {
                // Schauen ob die angegebene Mailadresse schon in der DB ist
                Customer existingCustomer = await _context.Customers.FirstOrDefaultAsync(m => m.Email.Trim() == customer.Email.Trim());

                // Wenn es schon einen Customer mit der Mailadresse gibt eine Warnung ausgeben das er schon existiert
                if (existingCustomer != null)
                {
                    TempData["UserExists"] = "Es existiert bereits ein Account mit diesen Daten!";
                    return View(customer);
                }

                await _userService.RegisterUserAsync(customer, password);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
