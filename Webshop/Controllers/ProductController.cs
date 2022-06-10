﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly lapWebshopContext _context;

        public ProductController(lapWebshopContext context)
        {
            _context = context;
        }

        public IActionResult Shop()
        {
            var products = _context.Products.Include(p => p.Category).Include(m => m.Manufacturer);

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = _context.Categories.ToList();

            foreach (var product in products)
            {
                product.NetUnitPrice = CalculateProductPrice.CalcPrice(product, categoryAndTaxRate);
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

        // GET: Products/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .Include(p => p.Category)
        //        .Include(p => p.Manufacturer)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Menge die ein Kunde maximal in den Warenkorb legen kann
            int amount = 10;

            Product product = _context.Products.Include(m => m.Manufacturer).SingleOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Locale Liste, würde sonst zu Problemen führen wenn es DbSet wäre, da zwei offene DB Verbindungen
            // bestehen würden
            List<Category> categoryAndTaxRate = _context.Categories.ToList();

            product.NetUnitPrice = CalculateProductPrice.CalcPrice(product, categoryAndTaxRate);

            List<SelectListItem> itemAmount = new List<SelectListItem>();

            for (int i = 1; i <= amount; i++)
            {
                itemAmount.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }

            ViewBag.ItemAmount = itemAmount;
            ViewBag.ImagePath = product.ImagePath;

            return View(product);
        }

        // GET: Products/Create
        //public IActionResult Create()
        //{
        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        //    ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name");
        //    return View();
        //}

        //// POST: Products/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ProductName,NetUnitPrice,ImagePath,Description,ManufacturerId,CategoryId")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        //    ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
        //    return View(product);
        //}

        //// GET: Products/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        //    ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
        //    return View(product);
        //}

        //// POST: Products/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,NetUnitPrice,ImagePath,Description,ManufacturerId,CategoryId")] Product product)
        //{
        //    if (id != product.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        //    ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
        //    return View(product);
        //}

        //// GET: Products/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .Include(p => p.Category)
        //        .Include(p => p.Manufacturer)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    _context.Products.Remove(product);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ProductExists(int id)
        //{
        //    return _context.Products.Any(e => e.Id == id);
        //}
    }
}
