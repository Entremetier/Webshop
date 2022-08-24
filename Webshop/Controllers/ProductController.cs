using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly LapWebshopContext _context;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly BewertungsService _bewertungsService;

        public ProductController(
            LapWebshopContext context,
            ProductService productService,
            CategoryService categoryService,
            BewertungsService bewertungsService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
            _bewertungsService = bewertungsService;
        }
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

                await _productService.IncreaseCounterByOne(product);

                ViewBag.Amount = itemAmount;
                ViewBag.ImagePath = product.ImagePath;
                ViewBag.Calls = await _productService.GetAmountOfProductCalls(product);

                List<Product> products = new List<Product>();
                foreach (var item in await _productService.GetProductsFromSameManufacturer(product))
                {
                    products.Add(item);
                } 

                foreach (var item in await _productService.GetProductsFromSameCategory(product))
                {
                    products.Add(item);
                }

                ProductWithListOfChoosenProduct productWithListOfChoosenProduct = new ProductWithListOfChoosenProduct
                    {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    NetUnitPrice = product.NetUnitPrice,
                    ImagePath = product.ImagePath,
                    Description = product.Description,
                    ManufacturerId = product.ManufacturerId,
                    CategoryId = product.CategoryId,
                    ProductList = products,
                    Comments = await _bewertungsService.GetComments(product.Id),
                    Punkte = await _bewertungsService.GetPoints(product.Id),
                    CustomerFirstChar = await _bewertungsService.GetCustomerFirstChar(await _bewertungsService.GetComments(product.Id))
                };

                return View(productWithListOfChoosenProduct);
            }
        }
    }
}
