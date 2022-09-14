using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class CustomerController : Controller
    {
        private readonly UserService _userService;

        public CustomerController(UserService userService)
        {
            _userService = userService;
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
            if (email == null || password == null)
            {
                TempData["LoginFailed"] = "E-Mail und Passwort angeben!";
                return RedirectToAction("Login");
            }
            //In der Datenbank prüfen ob es den Benutzer gibt und ob das Passwort stimmt
            var user = await _userService.CanUserLogInAsync(email.Trim(), password.Trim());

            if (user is null)
            {
                TempData["LoginFailed"] = "E-Mail oder Passwort ist falsch!";
                return RedirectToAction("Login");
            }

            // Mit email und user.Id die IdentityClaims (Behauptungen) des Users holen und Cookie mitgeben
            ClaimsIdentity claimsIdentity = _userService.GetClaimsIdentity(email, user);

            //Die Claims wandern in eine Identity, welche für den Principal (den Rechteinhaber) benötigt wird
            ClaimsPrincipal claimsPrincipal = _userService.GetClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(15)
                });

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
