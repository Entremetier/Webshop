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
    public class BewertungController : Controller
    {
        private readonly ProductService _productService;
        private readonly BewertungsService _bewertungsService;
        private readonly UserService _userService;
        public BewertungController(ProductService productService, BewertungsService bewertungsService, UserService userService)
        {
            _productService = productService;
            _bewertungsService = bewertungsService;
            _userService = userService;
        }

        public async Task<IActionResult> Bewertungen(int id)
        {
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt, user ist nicht eingeloggt, zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            // Customer aus der DB holen
            var customer = await _userService.GetCurrentUser(email);

            Product product = await _productService.GetProductWithManufacturerAndCategory(id);

            // Abfragen ob vom User eine Bewertung vorliegt bzw ob er es gekauft hat
            var bewertungen = await _bewertungsService.DidUserBuyProduct(customer, id);

            // Wenn der User das Produkt nicht gekauft oder schon bewertet hat zum Produkt schicken
            if (bewertungen != null)
            {
                TempData["DidntBuyItYet"] = "Sie müssen dieses Produkt erst kaufen um es zu bewerten. " +
                    "Doppelte Bewertungen sind nicht möglich.";
                return RedirectToAction("Details", "Product", new { id = id });
            }

            ViewBag.ImagePath = product.ImagePath;
            ViewBag.Punkte = _bewertungsService.GetPoints();

            return View(product);
        }

        public async Task<IActionResult> Bewerten(int id, string punkte, string comment)
        {
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt, user ist nicht eingeloggt, zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            // Customer aus der DB holen
            var customer = await _userService.GetCurrentUser(email);

            // Das gewählte Produkt aus DB holen
            var product = await _productService.GetProductWithManufacturerAndCategory(id);

            // Wenn es das Produkt nicht gibt
            if (product == null)
            {
                return RedirectToAction("Shop", "Home");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                comment = null;
            }

            await _bewertungsService.SetBewertung(id, punkte, comment, customer);

            return RedirectToAction("Shop", "Home");
        }
    }
}
