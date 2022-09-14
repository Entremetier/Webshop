using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly LapWebshopContext _context;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductController(
            LapWebshopContext context,
            ProductService productService,
            CategoryService categoryService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
        }

        // Get Product Details
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Home", "Shop");
            }
            else
            {
                Product product = await _productService.GetProductWithManufacturer(id.Value);

                if (product == null)
                {
                    return RedirectToAction("Home", "Shop");
                }

                var categoryAndTaxRate = await _categoryService.GetAllCategoriesAndTaxRates();

                // Auf 2 Nachkommastellen runden
                ViewBag.BruttoPrice = _productService.CalcPrice(product, categoryAndTaxRate);

                // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
                string email = User.FindFirstValue(ClaimTypes.Email);

                List<SelectListItem> itemAmount = await _productService.GetMaxItemAmount(product, email);

                if (itemAmount.Count == 0)
                {
                    TempData["EnoughItemsInCart"] = "Maximale Anzahl im Warenkorb!";
                }

                ViewBag.Amount = itemAmount;
                ViewBag.ImagePath = product.ImagePath;

                return View(product);
            }
        }
    }
}
