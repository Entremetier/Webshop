using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;
using Microsoft.EntityFrameworkCore;
using Webshop.ViewModels;
using Webshop.Controllers;

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

        public async Task SetOrder(Order order, List<OrderLine> orderLines, string firstName, string lastName, string street, string zip, string city)
        {
            List<Voucher> voucherList = new List<Voucher>();

            using (var db = new LapWebshopContext())
            {
                // Die OrderLines durchlaufen und schauen ob Gutscheine enthalten sind
                foreach (var item in orderLines)
                {
                    // Wenn Gutscheine enthalten sind die einzelnen Gutscheine erstellen
                    if (item.Product.ProductName == "Gutschein")
                    {
                        for (int i = 0; i < item.Amount; i++)
                        {
                            Voucher voucher = new Voucher();
                            voucher.Value = 200;
                            voucher.VoucherCode = CreateVoucherCode();
                            voucher.CustomerId = order.CustomerId;
                            voucherList.Add(voucher);
                        }
                    }
                }
                // Gutscheine in DB speichern
                await SaveListToDB(voucherList);

                // TODO: Seite erstellen um Gutscheine mit Code zu übermitteln
                //CreateVoucherFile(voucherList);


                // Order für den Customer speichern
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

        private async Task SaveListToDB(List<Voucher> voucherList)
        {
            using (var db = new LapWebshopContext())
            {
                // Liste mit Gutscheinen in DB speichern
                foreach (var item in voucherList)
                {
                    db.Vouchers.Add(item);
                }
                await db.SaveChangesAsync();
            }
        }

        // Methode um Gutscheincode zu ersellen
        public string CreateVoucherCode()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "0123456789";
            char[] stringChars = new char[10];
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                // ersten beiden Stellen als char
                if (i <= 1)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                // 2 Stellen als Zahlen
                else if (i > 1 && i <= 3)
                {
                    stringChars[i] = numbers[random.Next(numbers.Length)];
                }
                // 2 Stellen als char 
                else if (i > 3 && i <= 5)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                // 2 Stellen als Zahl 
                else if (i > 5 && i <= 7)
                {
                    stringChars[i] = numbers[random.Next(numbers.Length)];
                }
                // letzten 2 Stellen als char 
                else if (i > 7 && i <= 9)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
            }

            if (CheckIfVoucherCodeExists(stringChars.ToString()))
            {
                CreateVoucherCode();
            }

            return new string(stringChars);
        }

        // Kontrollieren das es nicht schon einen Gutschein in der DB mit dem Code gibt
        private bool CheckIfVoucherCodeExists(string voucherCode)
        {
            bool doesCodeExistInDb = false;

            using (var db = new LapWebshopContext())
            {
                var allVoucherCodes = db.Vouchers.Select(x => x.VoucherCode);

                // Kontrollieren ob der Code schon in der DB ist
                foreach (var code in allVoucherCodes)
                {
                    if (code == voucherCode)
                    {
                        doesCodeExistInDb = true;
                    }
                }
            }

            return doesCodeExistInDb;
        }

        public decimal GetFullNettoPriceOfOrderLines(List<OrderLine> orderLines)
        {
            decimal fullNettoPrice = 0;

            using (var db = new LapWebshopContext())
            {
                foreach (var orderLine in orderLines)
                {
                    fullNettoPrice += orderLine.Amount * orderLine.NetUnitPrice;
                }
            }

            return fullNettoPrice;
        }
    }
}
