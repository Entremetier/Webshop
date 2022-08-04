using Google.DataTable.Net.Wrapper;
using Google.DataTable.Net.Wrapper.Extension;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class StatisticController : Controller
    {
        private readonly ProductService _productService;

        public StatisticController(ProductService productService)
        {
            _productService = productService;
        }

        // TODO: Liste der Produkte holen und an die View übergeben
        public async Task<IActionResult> BestProductsWithGraph()
        {
            List<ProductsWithAmount> products = await _productService.GetProductsWithAmount();
            return View(products);
        }
    }
}
