using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserAccountService _userAccountService;


        public RegisterCustomerController(LapWebshopContext context, UserAccountService userAccountService)
        {
            _context = context;
            _userAccountService = userAccountService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterCustomer customer, string password)
        {
            //TODO: Kontrollieren ob es den User schon in der DB gibt

            // Im Model überprüfen ob die Einschränkungen bei den Feldern eingehalten wurden (z.B. max Länge von Feldern)
            // Wenn nicht wird die View mit customer und Fehlern angezeigt 
            if (ModelState.IsValid)
            {
                await _userAccountService.RegisterUserAsync(customer, password);
                return RedirectToAction("Index", "Home");
            }
            return View(customer);
        }
    }
}
