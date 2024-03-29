﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;
using Microsoft.EntityFrameworkCore;
using Webshop.ViewModels;

namespace Webshop.Services
{
    public class OrderService
    {
        public async Task<Order> GetOrder(Customer customer)
        {
            using (var db = new LapWebshopContext())
            {
                var order =  await db.Orders.Where(x => x.CustomerId == customer.Id)
                    .FirstOrDefaultAsync(e => e.DateOrdered == null);

                // Neue Order erstellen wenn keine vorhanden ist
                if (order == null)
                {
                    await CreateOrder(customer);

                    // Neu erstellte Order aus der DB holen
                    order = await GetOrder(customer);
                }
                return order;
            }
        }

        public async Task<Order> GetLastFinishedOrder(Customer customer)
        {
            using (var db = new LapWebshopContext())
            {
                var order = await db.Orders.Where(x => x.CustomerId == customer.Id)
                    .OrderBy(x => x)
                    .LastOrDefaultAsync(e => e.DateOrdered != null);

                return order;
            }
        }

        private async Task CreateOrder(Customer customer)
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

        public async Task SetOrder(Order order, string firstName, string lastName, string street, string zip, string city)
        {
            using (var db = new LapWebshopContext())
            {
                order.DateOrdered = DateTime.Now;
                order.FirstName = firstName;
                order.LastName = lastName;
                order.Street = street;
                order.Zip = zip;
                order.City = city;

                db.Update(order);
                await db.SaveChangesAsync();
            }
        }

        public decimal GetFullNettoPriceOfOrderLines(List<OrderLine> orderLines)
        {
            decimal fullNettoPrice = 0;
            foreach (var orderLine in orderLines)
            {
                fullNettoPrice += orderLine.Amount * orderLine.NetUnitPrice;
            }

            return fullNettoPrice;
        }
    }
}
