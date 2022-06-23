using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class OrderService
    {
        public Order GetOrder(Customer customer)
        {
            using (var db = new LapWebshopContext())
            {
                var order = db.Orders.Where(x => x.CustomerId == customer.Id)
                    .FirstOrDefault(e => e.DateOrdered == null);

                return order;
            }
        }

        public async void CreateOrder(Customer customer)
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
