using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly LapWebshopContext _context;
        private readonly Filter _filter;
        private readonly GetManufacturers _getManufacturers;
        private readonly GetCategories _getCategories;
        private readonly CalculateProductPrice _calculateProductPrice;

        public HomeController(LapWebshopContext context,
            Filter filter,
            GetManufacturers getManufacturers,
            GetCategories getCategories,
            CalculateProductPrice calculateProductPrice)
        {
            _context = context;
            _filter = filter;
            _getManufacturers = getManufacturers;
            _getCategories = getCategories;
            _calculateProductPrice = calculateProductPrice;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shop(string searchString, string categorie, string manufacturer)
        {
            //TODO: Filter speichern und ausführen wenn man zurück zur Liste geht
            //TODO: Filter löschen einbauen
            // Produktliste befüllen
            IQueryable<Product> products = _filter.FilterList(searchString, categorie, manufacturer);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = _context.Categories.ToList();

            // Bruttopreis für alle Produkte berechnen
            foreach (var product in products)
            {
                product.NetUnitPrice = _calculateProductPrice.CalcPrice(product, categoryAndTaxRate);
            }

            // Die DDL`s befüllen
            List<SelectListItem> allManufacturer = _getManufacturers.GetAllManufacturers();
            List<SelectListItem> allCategories = _getCategories.GetAllCategories();

            ViewBag.Manufacturers = allManufacturer;
            ViewBag.Category = allCategories;
            ViewBag.ProductsCount = products.Count();

            return View(products);
        }

        public async Task<IActionResult> ShoppingCart(string amount, int id)
        {
            using (var db = new LapWebshopContext())
            {
                // Den angemeldeten User mittels E-Mail-Claim identifizieren
                string email = User.FindFirstValue(ClaimTypes.Email);

                // Wenn es den User nicht gibt NotFound zurückgeben
                if (email == null)
                {
                    return RedirectToAction("Login", "Customer");
                }

                var customer = db.Customers.Where(e => e.Email == email).FirstOrDefault();

                int.TryParse(amount, out int amountInt);

                // Das gewählte Produkt aus DB holen
                var product = db.Products.Include(m => m.Manufacturer)
                    .Include(c => c.Category)
                    .FirstOrDefault(x => x.Id == id);

                // Gibt es das Produkt
                if (product == null)
                {
                    return NotFound();
                }

                //TODO: Gibt es einen offenen Warenkorb (Order)?
                //TODO: Gibt es das Produkt schon im Warenkorb (nur den amount erhöhen und db.Update und SaveChanges speichern)

                // Eine neue Order erstellen
                var newOrder = new Order
                {
                    CustomerId = customer.Id,
                    PriceTotal = 0,
                    //DateOrdered = DateTime.Today,
                    Street = customer.Street,
                    Zip = customer.Zip,
                    City = customer.City,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName
                };

                db.Orders.Add(newOrder);
                await db.SaveChangesAsync();

                var order = db.Orders.FirstOrDefault(e => e.DateOrdered == null);

                // Für die Offene Order des Users eine neue OrderLine hinzufügen
                var newOrderLine = new OrderLine
                {
                    ProductId = product.Id,
                    OrderId = order.Id,
                    Amount = amountInt,
                    NetUnitPrice = product.NetUnitPrice,
                    TaxRate = product.Category.TaxRate
                };

                // OrderLine speichern
                db.OrderLines.Add(newOrderLine);
                await db.SaveChangesAsync();

                // Die Seite nicht neu laden
                return RedirectToAction("Shop");
            }
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
