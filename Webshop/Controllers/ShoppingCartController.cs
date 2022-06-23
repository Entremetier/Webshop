using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class ShoppingCartController : Controller
    {
        public async Task<IActionResult> AddToShoppingCart(string amount, int id)
        {
            using (var db = new LapWebshopContext())
            {
                // Den angemeldeten User mittels E-Mail-Claim identifizieren
                string email = User.FindFirstValue(ClaimTypes.Email);

                // Wenn es den User nicht gibt NotFound zurückgeben
                if (email == null)
                {
                    return RedirectToAction("Login", "Customer");
                }

                // Customer aus der DB holen
                var customer = db.Customers.Where(e => e.Email == email)
                    .FirstOrDefault();

                int.TryParse(amount, out int amountInt);

                // Das gewählte Produkt aus DB holen
                var product = db.Products.Include(m => m.Manufacturer)
                    .Include(c => c.Category)
                    .FirstOrDefault(x => x.Id == id);

                // Wenn es das Produkt nicht gibt
                if (product == null)
                {
                    return NotFound();
                }

                // Für das Produkt den Bruttopreis berechnen
                List<Category> categoryAndTaxRate = db.Categories.ToList();
                //product.NetUnitPrice = _calculateProductPrice.CalcPrice(product, categoryAndTaxRate);

                // Die offene Bestellung des Users heraussuchen (DateOrdered == null)
                var order = db.Orders.Where(x => x.CustomerId == customer.Id)
                    .FirstOrDefault(e => e.DateOrdered == null);

                // Wenn es keine offene Order gibt
                if (order == null)
                {
                    // Eine neue Order erstellen, da es Produkt und Customer gibt
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

                    // Die neu erstellte, offene Bestellung des Users heraussuchen
                    order = db.Orders.Where(x => x.CustomerId == customer.Id)
                        .FirstOrDefault(e => e.DateOrdered == null);
                }

                // Im Warenkorb schauen ob es das Produkt mit der gesuchten ProduktId schon gibt
                var productAlreadyInCart = db.OrderLines.Where(x => x.ProductId == product.Id)
                    .FirstOrDefault();

                // Der offenen Order eine neue Zeile hinzufügen oder amount beim bestehenden Produkt erhöhen
                if (productAlreadyInCart != null)
                {
                    // Wenn es das Produkt schon im Warenkorb gibt, nur den amount erhöhen und db.Update
                    // und mit SaveChanges speichern.
                    productAlreadyInCart.Amount += amountInt;

                    db.Update(productAlreadyInCart);
                    await db.SaveChangesAsync();

                }
                else
                {
                    // Für die Offene Order des Users eine neue OrderLine hinzufügen
                    var newOrderLine = new OrderLine
                    {
                        ProductId = product.Id,
                        OrderId = order.Id,
                        Amount = amountInt,
                        NetUnitPrice = product.NetUnitPrice,
                        TaxRate = product.Category.TaxRate
                    };

                    // OrderLine speichern
                    db.OrderLines.Add(newOrderLine);
                    await db.SaveChangesAsync();
                }

                // Alle Produkte des Warenkorbs aus DB holen
                var productsInShoppingCart = db.OrderLines.Where(x => x.OrderId == order.Id && order.DateOrdered == null);
                decimal totalPrice = 0;

                // Alle Produkte den Bruttopreis berechnen und addieren um gesamtpreis in Order einzutragen
                foreach (var item in productsInShoppingCart)
                {
                    decimal itemOnePercent = item.NetUnitPrice / 100;
                    decimal itemTaxes = itemOnePercent * item.TaxRate;
                    decimal itemBruttoPrice = item.NetUnitPrice + itemTaxes;

                    decimal allItems = item.Amount * itemBruttoPrice;

                    totalPrice += allItems;
                }

                order.PriceTotal = totalPrice;

                db.Update(order);
                await db.SaveChangesAsync();

                // Die Seite nicht neu laden
                return RedirectToAction("Shop", "Home");
            }
        }
    }
}
