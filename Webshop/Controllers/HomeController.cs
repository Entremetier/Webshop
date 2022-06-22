﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly GetManufacturers _getManufacturers;
        private readonly GetCategories _getCategories;
        private readonly CalculateProductPrice _calculateProductPrice;

        private List<ShoppingCartModel> shoppingCart = new List<ShoppingCartModel>();


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

        public IActionResult ShoppingCart(string amount, int id)
        {
            ShoppingCartModel cartModel = new ShoppingCartModel();

            int.TryParse(amount, out int amountInt);

            var product = _context.Products.Include(m => m.Manufacturer)
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Bruttopreis für das Produkt berechnen
            var categoryAndTaxRate = _context.Categories.ToList();
            decimal bruttoUnitPrice = _calculateProductPrice.CalcPrice(product, categoryAndTaxRate);

            cartModel.Id = product.Id;
            cartModel.Amount = amountInt;
            cartModel.ProductName = product.ProductName;
            cartModel.ManufacturerName = product.Manufacturer.Name;
            cartModel.NetUnitPrice = product.NetUnitPrice;
            cartModel.BruttoUnitPrice = bruttoUnitPrice;
            cartModel.ImagePath = product.ImagePath;
            cartModel.Description = product.Description;

            shoppingCart.Add(cartModel);

            return RedirectToAction("Shop");
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
