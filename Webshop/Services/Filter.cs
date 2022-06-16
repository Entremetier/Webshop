using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class Filter
    {
        private readonly LapWebshopContext _context;

        public Filter(LapWebshopContext context)
        {
            _context = context;
        }

        public IQueryable<Product> FilterList(string searchString, string cat, string man)
        {
            IQueryable<Product> products = null;

            // cat und man können "0" sein wenn im DDL "Alle Kategorien/Hersteller" ausgewählt wird
            if (cat == "0")
            {
                cat = null;
            }

            if (man == "0")
            {
                man = null;
            }

            if (searchString == null && cat != null && man == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == cat);
            }
            else if (searchString != null && cat != null && man == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == cat && p.Description.Contains(searchString));
            }
            else if (searchString == null && cat == null && man != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Manufacturer.Name == man);
            }
            else if (searchString != null && cat == null && man != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Description.Contains(searchString) && p.Manufacturer.Name == man);
            }
            else if (searchString == null && cat != null && man != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == cat && p.Manufacturer.Name == man);
            }
            else if (searchString != null && cat != null && man != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == cat && p.Description.Contains(searchString) && p.Manufacturer.Name == man);
            }
            else if (searchString != null && cat == null && man == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Description.Contains(searchString) || p.ProductName.Contains(searchString) || p.Manufacturer.Name.Contains(searchString));
            }
            else
            {
                // Wenn die Liste beim Start befüllt wird oder bei der Suche keine Parameter angegben werden
                products = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer);
            }

            int count = products.Count();

            return products;
        }
    }
}
