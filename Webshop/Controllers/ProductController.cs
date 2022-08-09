using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly LapWebshopContext _context;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ManufacturerService _manufacturerService;


        public ProductController(
            LapWebshopContext context,
            ProductService productService,
            CategoryService categoryService,
            ManufacturerService manufacturerService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
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

        // Get Product Details
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Home", "Shop");
            }
            else
            {
                Product product = await _productService.GetProductWithManufacturer(id.Value);

                if (product == null)
                {
                    return RedirectToAction("Home", "Shop");
                }

                var categoryAndTaxRate = await _categoryService.GetAllCategoriesAndTaxRates();

                // Auf 2 Nachkommastellen runden
                ViewBag.BruttoPrice = _productService.CalcPrice(product, categoryAndTaxRate);

                // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
                string email = User.FindFirstValue(ClaimTypes.Email);

                List<SelectListItem> itemAmount = await _productService.GetMaxItemAmount(product, email);

                if (itemAmount.Count == 0)
                {
                    TempData["EnoughItemsInCart"] = "Maximale Anzahl im Warenkorb!";
                }

                ViewBag.Amount = itemAmount;
                ViewBag.ImagePath = product.ImagePath;

                return View(product);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            List<SelectListItem> allManufacturer = _manufacturerService.GetAllManufacturers();
            allManufacturer.Remove(allManufacturer.Where(x => x.Value == "0").Single());

            List<SelectListItem> allCategories = await _categoryService.GetAllCategories();
            allCategories.Remove(allCategories.Where(x => x.Value == "0").Single());

            ViewBag.Manufacturers = allManufacturer;
            ViewBag.Category = allCategories;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string productName, string netUnitPrice, IFormFile imageName, string description, string manufacturer, string categorie)
        {
            string imagePath = "";
            string dataBaseImagePath = "";

            if (
                string.IsNullOrWhiteSpace(productName) ||
                string.IsNullOrWhiteSpace(netUnitPrice) ||
                string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(manufacturer) ||
                string.IsNullOrWhiteSpace(categorie)
                )
            {
                TempData["Warning"] = "Fehler bei der Eingabe";
                return RedirectToAction("AddProduct");
            }

            if (imageName != null && imageName.Length > 0 && imageName.ContentType == "image/jpeg")
            {
                if (categorie == "Smartphone")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/src/images/smartphones/", imageName.FileName);
                    dataBaseImagePath = "~/src/images/smartphones/" + imageName.FileName;
                }
                else if (categorie == "Notebook")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/src/images/notebooks/", imageName.FileName);
                    dataBaseImagePath = "~/src/images/notebooks/" + imageName.FileName;
                }
                else if (categorie == "Tablet")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/src/images/tablets/", imageName.FileName);
                    dataBaseImagePath = "~/src/images/tablets/" + imageName.FileName;
                }
            }
            else
            {
                TempData["Warning"] = "Bildpfad kontrollieren";
                return RedirectToAction("AddProduct");
            }

            // Bild in den Ordner kopieren
            await _productService.AddImageToFolder(imagePath, imageName);

            // Produkt der Datenbank hinzufügen
            await _productService.AddProduct(productName, netUnitPrice, dataBaseImagePath, description, manufacturer, categorie);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _productService.GetProductWithManufacturerAndCategory(id.Value);

            if (product == null)
            {
                return RedirectToAction("Home", "Shop");
            }

            List<SelectListItem> allManufacturer = _manufacturerService.GetAllManufacturers();
            allManufacturer.Remove(allManufacturer.Where(x => x.Value == "0").Single());

            // DDL auf den aktuellen Wert setzen
            foreach (var item in allManufacturer)
            {
                if (item.Value == product.Manufacturer.Name)
                {
                    item.Selected = true;
                    break;
                }
            }

            List<SelectListItem> allCategories = await _categoryService.GetAllCategories();
            allCategories.Remove(allCategories.Where(x => x.Value == "0").Single());

            // DDL auf den aktuellen Wert setzen
            foreach (var item in allCategories)
            {
                if (item.Value == product.Category.Name)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Manufacturers = allManufacturer;
            ViewBag.Category = allCategories;

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProductDetails(int id, string productName, string netUnitPrice, string imagePath, string description, string manufacturer, string categorie)
        {
            if (
               string.IsNullOrWhiteSpace(productName) ||
               string.IsNullOrWhiteSpace(netUnitPrice) ||
               string.IsNullOrWhiteSpace(imagePath) ||
               string.IsNullOrWhiteSpace(description) ||
               string.IsNullOrWhiteSpace(manufacturer) ||
               string.IsNullOrWhiteSpace(categorie)
               )
            {
                TempData["Warning"] = "Eingabe kontrollieren";
                return RedirectToAction("ChangeProductDetails");
            }

            await _productService.ChangeProductDetails(id, productName, netUnitPrice, imagePath, description, manufacturer, categorie);
            return View();
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return RedirectToAction("Shop", "Home");
        }

        // GET: Product/Create
        //public IActionResult Create()
        //{
        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        //    ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name");
        //    return View();
        //}

        //// POST: Product/Create
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

        //// GET: Product/Edit/5
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

        //// POST: Product/Edit/5
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

        //// GET: Product/Delete/5
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

        //// POST: Product/Delete/5
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
