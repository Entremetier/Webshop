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
            Product product = await _productService.GetProductWithManufacturerAndCategory(id);

            TempData["UserNotLoggedIn"] = "Sie müssen eingeloggt sein um eine Bewertung abzugeben";
            ViewBag.ImagePath = product.ImagePath;
            ViewBag.Punkte = _bewertungsService.GetPoints();
            return View(product);
        }

        public async Task<IActionResult> Bewerten(int id, string valueUser, string? comment)
        {
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt, user ist nicht eingeloggt, zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            // TODO: Wenn der User das Produkt nicht gekauft hat Fehlermeldung schicken
            // Customer aus der DB holen
            var customer = await _userService.GetCurrentUser(email);

            // Das gewählte Produkt aus DB holen
            var product = await _productService.GetProductWithManufacturerAndCategory(id);

            // Wenn es das Produkt nicht gibt
            if (product == null)
            {
                return RedirectToAction("Shop", "Home");
            }
            ViewBag.DidntBuyItYet = _bewertungsService.DidUserBuyProduct();

            // TODO: Wenn der User das Produkt schon bewertet hat Fehlermeldung schicken
            // TODO: Wert vom user validieren
            // TODO: Values an Service übergeben um da in db zu schreiben

            return RedirectToAction("Shop", "Home");
        }
    }
}
