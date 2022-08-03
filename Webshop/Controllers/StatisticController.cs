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

        public async Task<IActionResult> BestProductsWithGraph()
        {
            List<ProductOfTheMonth> products = await _productService.GetProductOfTheMonth();
            return View(products);
        }
    }
}
