using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public class BewertungsService
    {
        // Die Punkte die vergeben werden können holen
        public List<SelectListItem> GetPoints()
        {
            List<SelectListItem> points = new List<SelectListItem>();
            int maxPoints = 5;
            for (int i = 1; i <= maxPoints; i++)
            {
                points.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }

            return points;
        }

        // TODO: Hat User das Produkt gekauft
        //public Task DidUserBuyProduct()
    }
}
