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
        private readonly Filter _filter;

        public HomeController(LapWebshopContext context, Filter filter)
        {
            _context = context;
            _filter = filter;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shop(string? searchString, string? cat, string? man)
        {
            // Produktliste befüllen
            IQueryable<Product> products = _filter.FilterList(searchString, cat, man);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = _context.Categories.ToList();

            foreach (var product in products)
            {
                product.NetUnitPrice = CalculateProductPrice.CalcPrice(product, categoryAndTaxRate);
            }

            var manufacturers = _context.Manufacturers.Select(val => val.Name);

            List<SelectListItem> allManufacturer = new List<SelectListItem>();
            allManufacturer.Add(new SelectListItem { Value = "0", Text = "Alle Hersteller" });

            foreach (var item in manufacturers)
            {
                allManufacturer.Add(new SelectListItem { Value = item.ToString(), Text = item.ToString() });
            }

            var categories = _context.Categories.Select(val => val.Name);

            List<SelectListItem> allCategories = new List<SelectListItem>();
            allCategories.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });

            foreach (var item in categories)
            {
                allCategories.Add(new SelectListItem { Value = item.ToString(), Text = item.ToString() });
            }

            ViewBag.Manufacturers = allManufacturer;
            ViewBag.Category = allCategories;
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
