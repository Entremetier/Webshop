using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class CategoryService
    {
        public List<Category> GetAllCategoriesAndTaxRates()
        {
            using (var db = new LapWebshopContext())
            {
                List<Category> categoryAndTaxRate = db.Categories.ToList();
                return categoryAndTaxRate;
            }
        }
    }
}
