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
        public List<SelectListItem> GetAllCategories()
        {
            List<SelectListItem> allCategories = new List<SelectListItem>();

            using (var db = new LapWebshopContext())
            {
                // Alle Kategorien, alphabetisch geordnet
                var categories = db.Categories.Select(c => c.Name).OrderBy(c => c);
                allCategories.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });

                foreach (var category in categories)
                {
                    allCategories.Add(new SelectListItem { Value = category.ToString(), Text = category.ToString() });
                }

                return allCategories;
            }
        }
    }
}
