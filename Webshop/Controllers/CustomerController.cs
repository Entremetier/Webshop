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

        // GET: Customer
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Customers.ToListAsync());
        //}

        //// GET: Customer/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = await _context.Customers
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customer);
        //}

        // GET: Customer/Create
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CustomerLogin customer, string password)
        //{
        //    //TODO: Kontrollieren ob es den User schon in der DB gibt

        //    // Im Model überprüfen ob die Einschränkungen bei den Feldern eingehalten wurden (z.B. max Länge von Feldern)
        //    // Wenn nicht wird die View mit customer und Fehlern angezeigt 
        //    if (ModelState.IsValid)
        //    {
        //        await _userAccountService.RegisterUserAsync(customer, password);
        //        return RedirectToAction("Home/Shop");
        //    }
        //    return View(customer);
        //}

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
            ClaimsIdentity claimsIdentity = _userService.GetClaimsIdentity(email, user.Id);

            //Die Claims wandern in eine Identity, welche für den Principal (den Rechteinhaber) benötigt wird
            ClaimsPrincipal claimsPrincipal = _userService.GetClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                new AuthenticationProperties
                {
                IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(5)
                });

            return RedirectToAction("Shop", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Customer/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = await _context.Customers.FindAsync(id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(customer);
        //}

        //// POST: Customer/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Title,FirstName,LastName,Email,Street,Zip,City,PwHash,Salt")] Customer customer)
        //{
        //    if (id != customer.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(customer);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CustomerExists(customer.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(customer);
        //}

        //// GET: Customer/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = await _context.Customers
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customer);
        //}

        //// POST: Customer/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var customer = await _context.Customers.FindAsync(id);
        //    _context.Customers.Remove(customer);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CustomerExists(int id)
        //{
        //    return _context.Customers.Any(e => e.Id == id);
        //}
    }
}
