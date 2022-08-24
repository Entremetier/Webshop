using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

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
        public async Task<Bewertungen> DidUserBuyProduct(Customer customer, int productId)
        {
            using (var db = new LapWebshopContext())
            {
                var bewertetesProdukt = await db.Bewertungens.Where(x => x.CustomerId == customer.Id && x.ProductId == productId)
                    .FirstOrDefaultAsync();
                return bewertetesProdukt;
            }
        }

        public async Task SetBewertung(int id, string punkte, string comment, Customer customer)
        {
            using (var db = new LapWebshopContext())
            {
                Bewertungen bewertungen = new Bewertungen()
                {
                    ProductId = id,
                    CustomerId = customer.Id,
                    Comment = comment,
                    Points = Int16.Parse(punkte)
                };

                await db.AddAsync(bewertungen);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetComments(int productId)
        {
            using (var db = new LapWebshopContext())
            {
                // Kommentare suchen, leere nicht übernehmen
                var comments = await db.Bewertungens.Where(x => x.ProductId == productId && x.Comment != null)
                    .Select(x => x.Comment)
                    .ToListAsync();

                return comments;
            }
        }

        public async Task<int> GetPoints(int productId)
        {
            using (var db = new LapWebshopContext())
            {
                var points = await db.Bewertungens.Where(x => x.ProductId == productId)
                    .Select(x => x.Points.Value)
                    .ToListAsync();

                // Wenn das Produkt noch nicht bewertet wurde eine Punkt vergeben
                if (points.Count() == 0)
                {
                    points.Add(1);
                }

                int allPoints = 0;
                foreach (var point in points)
                {
                    allPoints += point;
                }

                return allPoints / points.Count();
            }
        }

        public async Task<string> GetCustomerFirstChar(List<string> comments)
        {
            using (var db = new LapWebshopContext())
            {
                var usersFirstChar = await db.Bewertungens.Include(x => x.Customer)
                    .Where(x => x.Customer.Id == x.CustomerId)
                    .Select(x => x.Customer.FirstName)
                    .FirstOrDefaultAsync();


                return usersFirstChar.Substring(0,1);
            }
        }
    }
}
