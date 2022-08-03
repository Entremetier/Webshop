﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class ProductService
    {
        // Über _context arbeiten damit die Verbindung, beim filtern, zur DB nicht zu früh geschlossen wird
        private readonly LapWebshopContext _context;
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public ProductService(LapWebshopContext context, UserService userService, OrderService orderService)
        {
            _context = context;
            _orderService = orderService;
            _userService = userService;
        }
        public async Task<Product> GetProductWithManufacturerAndCategory(int id)
        {
            using (var db = new LapWebshopContext())
            {
                var product = await db.Products.Include(m => m.Manufacturer)
                    .Include(c => c.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return product;
            }
        }

        public async Task<Product> GetProductWithManufacturer(int id)
        {
            using (var db = new LapWebshopContext())
            {
                Product product = await db.Products
                    .Include(m => m.Manufacturer)
                    .FirstOrDefaultAsync(p => p.Id == id);

                return product;
            }
        }

        public decimal CalcPrice(Product product, List<Category> categoryAndTaxRate)
        {
            decimal itemBruttoPrice = 0;

            foreach (var cat in categoryAndTaxRate)
            {
                if (product.CategoryId == cat.Id)
                {
                    itemBruttoPrice = product.NetUnitPrice / 100 * (100 + cat.TaxRate);
                }
            }
            // Auf 2 Nachkommastellen runden
            return Math.Round(itemBruttoPrice, 2);
        }

        public async Task<List<SelectListItem>> GetMaxItemAmount(Product product, string email)
        {
            List<SelectListItem> itemAmount = new List<SelectListItem>();

            using (var db = new LapWebshopContext())
            {
                // Wenn user angemeldet ist
                if (email != null)
                {
                    //User aus DB holen
                    Customer customer = await _userService.GetCurrentUser(email);

                    // Die offene Bestellung des Users aus DB holen
                    var order = await _orderService.GetOrder(customer);

                    // Menge vom Product die schon im Warenkorb ist
                    int productAmountInCart = await db.OrderLines.Where(x => x.ProductId == product.Id && order.Id == x.OrderId && order.DateOrdered == null)
                        .Select(x => x.Amount)
                        .FirstOrDefaultAsync();

                    // Wenn schon 10 Stk von einem Product im Warenkorb sind
                    if (productAmountInCart >= MaxItemsInCart.MaxItemsInShoppingCart)
                    {
                        //itemAmount.Add(new SelectListItem { Value = "0", Text = "0" });
                    }
                    else
                    {
                        int amountCustomerCanAdd = MaxItemsInCart.MaxItemsInShoppingCart - productAmountInCart;
                        for (int i = 1; i <= amountCustomerCanAdd; i++)
                        {
                            itemAmount.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                        }
                    }
                }
                // Wenn user nicht angemeldet ist
                else
                {
                    for (int i = 1; i <= MaxItemsInCart.MaxItemsInShoppingCart; i++)
                    {
                        itemAmount.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                    }
                }
                return itemAmount;
            }
        }

        //TODO: Den amount aller produkte mit der gleichen ID zusammenzählen, am ende das Produkt mit den meisten verkäufen zurückgeben (id zurückgeben)
        public async Task<List<ProductOfTheMonth>> GetProductOfTheMonth()
        {
            using (var db = new LapWebshopContext())
            {
                int amountOfDays = 30;

                // Alle Produkte aus OrderLines holen die schon bestellt wurden
                var productList = await db.OrderLines
                    .OrderBy(p => p.ProductId)
                    .Select(p => new ProductOfTheMonth { ProductId = p.ProductId, Amount = p.Amount, DateOrdered = p.Order.DateOrdered.Value })
                    .Where(x => x.DateOrdered.Year == DateTime.Now.Year && x.DateOrdered > DateTime.Now.AddDays(-amountOfDays))
                    .ToListAsync();

                // Die Summe aller Produkte zusammenrechnen und absteigend sortieren
                var products = productList
                    .GroupBy(x => x.ProductId)
                    .Select(prod => new ProductOfTheMonth { ProductId = prod.Key, Amount = prod.Sum(x => x.Amount) })
                    .OrderByDescending(x => x.Amount)
                    .ToList();

                return products;
            }
        }

        public IQueryable<Product> FilterList(string searchString, string categorie, string manufacturer)
        {
            IQueryable<Product> products = null;

            // categorie und manufacturer können "0" sein wenn im DDL "Alle Kategorien/Hersteller" ausgewählt wird
            if (categorie == "0")
            {
                categorie = null;
            }

            if (manufacturer == "0")
            {
                manufacturer = null;
            }

            if (searchString == null && categorie != null && manufacturer == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == categorie);
            }
            else if (searchString != null && categorie != null && manufacturer == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == categorie && p.ProductName.Contains(searchString) ||
                    p.Category.Name == categorie && p.Manufacturer.Name.Contains(searchString) ||
                    p.Category.Name == categorie && p.Description.Contains(searchString));
            }
            else if (searchString == null && categorie == null && manufacturer != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Manufacturer.Name == manufacturer);
            }
            else if (searchString != null && categorie == null && manufacturer != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.ProductName.Contains(searchString) && p.Manufacturer.Name == manufacturer ||
                    p.Manufacturer.Name.Contains(searchString) && p.Manufacturer.Name == manufacturer ||
                    p.Description.Contains(searchString) && p.Manufacturer.Name == manufacturer);
            }
            else if (searchString == null && categorie != null && manufacturer != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == categorie && p.Manufacturer.Name == manufacturer);
            }
            else if (searchString != null && categorie != null && manufacturer != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == categorie && p.Manufacturer.Name == manufacturer && p.ProductName.Contains(searchString) ||
                    p.Category.Name == categorie && p.Manufacturer.Name == manufacturer && p.Manufacturer.Name.Contains(searchString) ||
                    p.Category.Name == categorie && p.Manufacturer.Name == manufacturer && p.Description.Contains(searchString));
            }
            else if (searchString != null && categorie == null && manufacturer == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Description.Contains(searchString) || p.ProductName.Contains(searchString) || p.Manufacturer.Name.Contains(searchString));
            }
            else
            {
                // Wenn die Liste beim Start befüllt wird oder bei der Suche keine Parameter angegeben werden
                products = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer);
            }

            return products;
        }
    }
}
