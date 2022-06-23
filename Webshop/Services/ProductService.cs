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
        public Product GetProduct(int id)
        {
            using (var db = new LapWebshopContext())
            {
                var product = db.Products.Include(m => m.Manufacturer)
                    .Include(c => c.Category)
                    .FirstOrDefault(x => x.Id == id);

                return product;
            }
        }
    }
}
