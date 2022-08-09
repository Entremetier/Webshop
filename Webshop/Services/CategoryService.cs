using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Webshop.Services
{
    public class CategoryService
    {
        public async Task<List<Category>> GetAllCategoriesAndTaxRates()
        {
            using (var db = new LapWebshopContext())
            {
                List<Category> categoryAndTaxRate = await db.Categories.ToListAsync();
                return categoryAndTaxRate;
            }
        }

        public async Task<List<SelectListItem>> GetAllCategories()
        {
            List<SelectListItem> allCategories = new List<SelectListItem>();

            using (var db = new LapWebshopContext())
            {
                // Alle Kategorien, alphabetisch geordnet
                List<string> categories = await db.Categories.Select(c => c.Name).OrderBy(c => c).ToListAsync();
                allCategories.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });

                foreach (var category in categories)
                {
                    allCategories.Add(new SelectListItem { Value = category.ToString(), Text = category.ToString() });
                }

                return allCategories;
            }
        }

        public int GetCategoryId(string categoryName)
        {
            using (var db = new LapWebshopContext())
            {
                return db.Categories.Where(x => x.Name == categoryName).Select(x => x.Id).FirstOrDefault();
            }
        }
    }
}
