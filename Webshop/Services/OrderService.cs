using System;
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
                var order = await db.Orders.Where(x => x.CustomerId == customer.Id)
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

        public async Task<decimal> GetFullNettoPriceOfOrderLines(List<OrderLine> orderLines)
        {
            decimal fullNettoPrice = 0;
            List<OrderLine> voucherList = new List<OrderLine>();

            using (var db = new LapWebshopContext())
            {
                // TODO: Den Gutschein des Users aus der DB holen (Methode in ProductService)
                //voucherId = await db.Products.Where(x => x.ProductName == "Gutschein").Select(x => x.Id).FirstOrDefaultAsync();

                // Liste mit Gutscheinen anlegen
                //foreach (var orderline in orderLines)
                //{
                //    if (orderline.Id == voucherId)
                //    {
                //        voucherList.Add(orderline);
                //        orderLines.Remove(orderline);
                //    }
                //}

                // Gesamtpreis ohne Gutscheine berechnen
                foreach (var orderLine in orderLines)
                {
                    fullNettoPrice += orderLine.Amount * orderLine.NetUnitPrice;
                }

                // TODO: Vom fullNettoPrice Geld abziehen bis Gutschein leer ist oder fullNettoPrice = 0
                //foreach (var orderLineVoucher in voucherList)
                //{
                //    if (fullNettoPrice > 0)
                //    {
                //        decimal voucherValue = orderLineVoucher.Amount * orderLineVoucher.NetUnitPrice;
                //        fullNettoPrice -= voucherValue;

                //        // TODO: Gutscheinen den neuen Wert geben, wenn fullNettoPrice < 0 dann wieder gutschreiben
                //        if (fullNettoPrice < 0)
                //        {

                //            break;
                //        }
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}

                // TODO: fullNettoPrice kann nicht kleiner als 0 sein (eventuellen Übertrag wieder auf Gutschein schreiben)
                //if (fullNettoPrice < 0)
                //{
                //    betragFuerGutscheinGutschrift = 
                //}

                // TODO: Gutschein wieder in der Datenbank mit neuen Werten speichern
            }


            return fullNettoPrice;
        }
    }
}
