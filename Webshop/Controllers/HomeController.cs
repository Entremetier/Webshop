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
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly ManufacturerService _manufacturerService;
        private readonly UserService _userService;
        private readonly OrderLineService _orderLineService;
        private readonly OrderService _orderService;

        public HomeController(
            CategoryService categoryService,
            ProductService productService,
            ManufacturerService manufacturerService,
            UserService userService,
            OrderLineService orderLineService,
            OrderService orderService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _manufacturerService = manufacturerService;
            _userService = userService;
            _orderLineService = orderLineService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Shop(string searchString, string categorie, string manufacturer)
        {
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            //string email = User.FindFirstValue(ClaimTypes.Email);

            // Customer aus der DB holen
            //var customer = await _userService.GetCurrentUser(email);

            // Wenn der Customer angemeldet ist seine offene Order suchen oder eine neue erstellen
            //if (customer != null)
            //{
            //    var order = await _orderService.GetOrder(customer);
            //    ViewBag.OrderLines = await _orderLineService.GetOrderLinesOfOrder(order);
            //}

            // Produktliste befüllen
            IQueryable<Product> products = _productService.FilterList(searchString, categorie, manufacturer);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = await _categoryService.GetAllCategoriesAndTaxRates();

            // Bruttopreis für alle Produkte berechnen
            foreach (var product in products)
            {
                product.NetUnitPrice = _productService.CalcPrice(product, categoryAndTaxRate);
            }

            // Die DDL`s befüllen
            List<SelectListItem> allManufacturer = _manufacturerService.GetAllManufacturersAsSelectListItem();

            foreach (var item in allManufacturer)
            {
                if (item.Value == manufacturer)
                {
                    item.Selected = true;
                    break;
                }
            }
            List<SelectListItem> allCategories = await _categoryService.GetAllCategories();

            foreach (var item in allCategories)
            {
                if (item.Value == categorie)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.MaxItems = MaxItemsInCart.MaxItemsInShoppingCart;
            ViewBag.Manufacturers = allManufacturer;
            ViewBag.Category = allCategories;
            ViewBag.ProductsCount = products.Count();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Shop()
        {
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            //string email = User.FindFirstValue(ClaimTypes.Email);

            // Customer aus der DB holen
            //var customer = await _userService.GetCurrentUser(email);

            // Wenn der Customer angemeldet ist seine offene Order suchen oder eine neue erstellen
            //if (customer != null)
            //{
            //    var order = await _orderService.GetOrder(customer);
            //    ViewBag.OrderLines = await _orderLineService.GetOrderLinesOfOrder(order);
            //}

            // Produktliste befüllen
            IQueryable<Product> products = _productService.FilterList(null, null, null);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = await _categoryService.GetAllCategoriesAndTaxRates();

            // Bruttopreis für alle Produkte berechnen
            foreach (var product in products)
            {
                product.NetUnitPrice = _productService.CalcPrice(product, categoryAndTaxRate);
            }

            // Die DDL`s befüllen
            List<SelectListItem> allManufacturer = _manufacturerService.GetAllManufacturersAsSelectListItem();
            List<SelectListItem> allCategories = await _categoryService.GetAllCategories();

            ViewBag.MaxItems = MaxItemsInCart.MaxItemsInShoppingCart;
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
