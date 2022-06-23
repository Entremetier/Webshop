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
        private readonly OrderService _orderService;

        public ProductService(OrderService orderService)
        {
            _orderService = orderService;
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
    }
}
