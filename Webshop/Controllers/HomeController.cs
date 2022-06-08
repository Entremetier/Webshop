using Microsoft.AspNetCore.Mvc;
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

            ViewBag.ProductsCount = products.Count();
            //ViewBag.Path = 

            return View(products);
        }

        public IActionResult Details(int id)
        {
            Product product = ctx.Products.Include(m => m.Manufacturer).SingleOrDefault(p => p.Id == id);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = ctx.Categories.ToList();

            product.NetUnitPrice = CalcPrice(product, categoryAndTaxRate);

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
