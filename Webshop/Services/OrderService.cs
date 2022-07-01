using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Webshop.Services
{
    public class OrderService
    {
        public async Task<Order> GetOrder(Customer customer)
        {
            using (var db = new LapWebshopContext())
            {
                var order = await db.Orders.Where(x => x.CustomerId == customer.Id)
                    .FirstOrDefaultAsync(e => e.DateOrdered == null);

                // Neue Order erstellen wenn keine vorhanden ist
                if (order == null)
                {
                    CreateOrder(customer);

                     // Neu erstellte Order aus der DB holen
                    order = await GetOrder(customer);
                }
                return order;
            }
        }

        private async void CreateOrder(Customer customer)
        {
            using (var db = new LapWebshopContext())
            {
                var newOrder = new Order
                {
                    CustomerId = customer.Id,
                    PriceTotal = 0,
                    Street = customer.Street,
                    Zip = customer.Zip,
                    City = customer.City,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName
                };

                db.Orders.Add(newOrder);
                await db.SaveChangesAsync();
            }
        }
    }
}
