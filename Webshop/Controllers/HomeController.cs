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
        private readonly LapWebshopContext _context;

        public HomeController(LapWebshopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shop(string? searchString)
        {
            // Produktliste befüllen
            IQueryable<Product> products = FilterList(searchString);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = _context.Categories.ToList();

            foreach (var product in products)
            {
                product.NetUnitPrice = CalculateProductPrice.CalcPrice(product, categoryAndTaxRate);
            }

            var manufacturers = _context.Manufacturers.Select(val => val.Name);

            List<SelectListItem> manufacturer = new List<SelectListItem>();
            manufacturer.Add(new SelectListItem { Value = "0", Text = "Alle Hersteller" });

            foreach (var item in manufacturers)
            {
                manufacturer.Add(new SelectListItem { Value = item.ToString(), Text = item.ToString() });
            }

            var categories = _context.Categories.Select(val => val.Name);

            List<SelectListItem> allCategories = new List<SelectListItem>();
            allCategories.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });

            foreach (var item in categories)
            {
                allCategories.Add(new SelectListItem { Value = item.ToString(), Text = item.ToString() });
            }

            ViewBag.Manufacturers = manufacturer;
            ViewBag.Category = allCategories;
            ViewBag.ProductsCount = products.Count();


            return View(products);
        }

        public IQueryable<Product> FilterList(string searchString)
        {
            // List<Product> products = new List<Product>();
            IQueryable<Product> products = null;

            if (searchString == null)
            {
                products = _context.Products
                    .Include(p => p.Category)
                    .Include(m => m.Manufacturer);
            }
            else
            {
                products = _context.Products
                    .Include(m => m.Manufacturer)
                    .Where(x => x.Manufacturer.Name == searchString);
            }

            int count = products.Count();

            return products;
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
