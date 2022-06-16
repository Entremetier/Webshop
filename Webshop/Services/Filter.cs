﻿using Microsoft.EntityFrameworkCore;
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

        public IQueryable<Product> FilterList(string searchString, string categorie, string manufacturer)
        {
            IQueryable<Product> products = null;

            // cat und man können "0" sein wenn im DDL "Alle Kategorien/Hersteller" ausgewählt wird
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
                    .Where(p => p.Category.Name == categorie && p.Description.Contains(searchString));
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
                    .Where(p => p.Description.Contains(searchString) && p.Manufacturer.Name == manufacturer);
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
                    .Where(p => p.Category.Name == categorie && p.Description.Contains(searchString) && p.Manufacturer.Name == manufacturer);
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
