using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class ManufacturerService
    {
        private readonly LapWebshopContext _context;

        public ManufacturerService(LapWebshopContext context)
        {
            _context = context;
        }
        public List<SelectListItem> GetAllManufacturers()
        {
            List<SelectListItem> allManufacturer = new List<SelectListItem>();

            // Alle Hersteller, alphabetisch geordnet
            var manufacturers = _context.Manufacturers.Select(m => m.Name).OrderBy(m => m);

            allManufacturer.Add(new SelectListItem { Value = "0", Text = "Alle Hersteller" });

            foreach (var manufacturer in manufacturers)
            {
                allManufacturer.Add(new SelectListItem { Value = manufacturer.ToString(), Text = manufacturer.ToString() });
            }

            return allManufacturer;
        }
    }
}
