using Microsoft.AspNetCore.Mvc.Rendering;
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

        public ProductService(LapWebshopContext context)
        {
            _context = context;
        }
        public Product GetProductWithManufacturerAndCategory(int id)
        {
            using (var db = new LapWebshopContext())
            {
                var product = db.Products.Include(m => m.Manufacturer)
                    .Include(c => c.Category)
                    .FirstOrDefault(x => x.Id == id);

                return product;
            }
        }

        public Product GetProductWithManufacturer(int id)
        {
            using (var db = new LapWebshopContext())
            {
                Product product = db.Products
                    .Include(m => m.Manufacturer)
                    .FirstOrDefault(p => p.Id == id);

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
            return itemBruttoPrice;
        }

        public List<SelectListItem> GetMaxItemAmount(Product product)
        {
            // Menge die ein Kunde maximal in den Warenkorb legen kann
            int maxAmountOfItems = 10;

            List<SelectListItem> itemAmount = new List<SelectListItem>();

            using (var db = new LapWebshopContext())
            {
                // TODO: Abfrage anpassen um richtiges Ergebnis zurück zu bekommen
                int productAmountInCart = db.OrderLines.Include(x => x.Order)
                    .Where(x => x.ProductId == product.Id && x.Order.DateOrdered == null)
                    .Select(x => x.Amount)
                    .FirstOrDefault();

                if (productAmountInCart == 0)
                {
                    productAmountInCart++;
                }

                if (productAmountInCart >= maxAmountOfItems)
                {
                    itemAmount.Add(new SelectListItem { Value = "0", Text = "0" });
                }
                else
                {
                    // TODO: Berechnung um die noch fehlenden Werte in das DDL hinzufügen (z.B. 1-4, wenn schon 6 Stk im Warenkorb sind)
                    for (int i = 1; i <= maxAmountOfItems; i++)
                    {
                        itemAmount.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                    }
                }

                return itemAmount;
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
