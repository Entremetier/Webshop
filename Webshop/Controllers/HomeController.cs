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

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly lapWebshopContext ctx = new lapWebshopContext();


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shop()
        {
            var products = ctx.Products.Include(p => p.Category).Include(m => m.Manufacturer);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = ctx.Categories.ToList();

            foreach (var product in products)
            {
                product.NetUnitPrice = CalcPrice(product, categoryAndTaxRate);
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

        public IActionResult Details(int id)
        {
            // Menge die ein Kunde maximal in den Warenkorb legen kann
            int amount = 10;

            Product product = ctx.Products.Include(m => m.Manufacturer).SingleOrDefault(p => p.Id == id);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre, da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = ctx.Categories.ToList();

            product.NetUnitPrice = CalcPrice(product, categoryAndTaxRate);

            List<SelectListItem> itemAmount = new List<SelectListItem>();

            for (int i = 1; i <= amount; i++)
            {
                itemAmount.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }

            ViewBag.ItemAmount = itemAmount;
            ViewBag.ImagePath = product.ImagePath;

            return View(product);
        }

        private decimal CalcPrice(Product product, List<Category> categoryAndTaxRate)
        {
            decimal price = 0;

            foreach (var cat in categoryAndTaxRate)
            {
                if (product.CategoryId == cat.Id)
                {
                    decimal netPriceDividedBy100 = product.NetUnitPrice / 100;
                    decimal taxes = netPriceDividedBy100 * cat.TaxRate;
                    price = product.NetUnitPrice + taxes;
                }
            }
            return price;
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
