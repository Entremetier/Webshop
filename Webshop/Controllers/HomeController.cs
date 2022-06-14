using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly lapWebshopContext _context;

        public HomeController(lapWebshopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shop()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(m => m.Manufacturer);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = _context.Categories.ToList();

            foreach (var product in products)
            {
                product.NetUnitPrice = CalculateProductPrice.CalcPrice(product, categoryAndTaxRate);
            }

            List<SelectListItem> filters = new()
            {
                new SelectListItem { Value = "1", Text = "Hersteller" },
                new SelectListItem { Value = "2", Text = "Kategorie" },
                new SelectListItem { Value = "3", Text = "Produktname" }
            };

            ViewBag.Filters = filters;
            ViewBag.ProductsCount = products.Count();


            return View(products);
        }

        public IActionResult Impressum()
        {
            return View();
        }

        public IActionResult Agb()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
