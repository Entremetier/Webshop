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
    public class CustomerLoginController : Controller
    {
        private readonly LapWebshopContext _context;
        private readonly UserAccountService _userAccountService;


        public CustomerLoginController(LapWebshopContext context, UserAccountService userAccountService)
        {
            _context = context;
            _userAccountService = userAccountService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerLogin customer, string password)
        {
            //TODO: Kontrollieren ob es den User schon in der DB gibt

            // Im Model überprüfen ob die Einschränkungen bei den Feldern eingehalten wurden (z.B. max Länge von Feldern)
            // Wenn nicht wird die View mit customer und Fehlern angezeigt 
            if (ModelState.IsValid)
            {
                await _userAccountService.RegisterUserAsync(customer, password);
                return RedirectToAction("Home/Shop");
            }
            return View(customer);
        }

        // GET: Customer/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Customer/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            //In der Datenbank prüfen ob es den Benutzer gibt und ob das Passwort stimmt
            var user = await _userAccountService.CanUserLogInAsync(email, password);

            if (user is null) return RedirectToAction("Index", "Home");

            UserSignIn userSign = new UserSignIn();

            // Mit email und user.Id die IdentityClaims (Behauptungen) des Users holen und Cookie mitgeben
            ClaimsIdentity claimsIdentity = userSign.GetClaimsIdentity(email, user.Id);

            //Die Claims wandern in eine Identity, welche wir für den Principal (den Rechteinhaber) benötigen
            ClaimsPrincipal claimsPrincipal = userSign.GetClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Shop", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
