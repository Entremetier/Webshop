using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class GetCategories
    {
        private readonly LapWebshopContext _context;

        public GetCategories(LapWebshopContext context)
        {
            _context = context;
        }
        public List<SelectListItem> GetAllCategories()
        {
            List<SelectListItem> allCategories = new List<SelectListItem>();

            var categories = _context.Categories.Select(c => c.Name);
            allCategories.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });

            foreach (var category in categories)
            {
                allCategories.Add(new SelectListItem { Value = category.ToString(), Text = category.ToString() });
            }

            return allCategories;
        }
    }
}
