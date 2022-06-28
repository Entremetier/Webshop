using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class OrderLineService : Controller
    {
        public void GetProductIfInShoppingCart(Product product, Order order, int amount)
        {
            using (var db = new LapWebshopContext())
            {
                // Kontrollieren ob ein Produkt schon im Warenkorb, des angemeldeten users, ist
                var productAlreadyInCart = db.OrderLines.Where(x => x.ProductId == product.Id && order.Id == x.OrderId && order.DateOrdered == null)
                    .FirstOrDefault();

                if (productAlreadyInCart != null)
                {
                    if (productAlreadyInCart.Amount >= new MaxItemsInCart().MaxItemsInShoppingCart)
                    {
                        return;
                    }

                    IncrementAmountOfProduct(productAlreadyInCart, order, amount);
                }
                else
                {
                    CreateNewOrderLine(product, order, amount);
                }
            }
        }

        private async void IncrementAmountOfProduct(OrderLine productAlreadyInCart, Order order, int amount)
        {
            using (var db = new LapWebshopContext())
            {
                // Menge des vorhandenen Produktes im Warenkorb erhöhen
                productAlreadyInCart.Amount += amount;

                db.Update(productAlreadyInCart);
                await db.SaveChangesAsync();

                UpdateOrderTotalPrice(order);
            }
        }

        private async void CreateNewOrderLine(Product product, Order order, int amount)
        {
            using (var db = new LapWebshopContext())
            {
                // Für die Offene Order des Users eine neue OrderLine hinzufügen
                var newOrderLine = new OrderLine
                {
                    ProductId = product.Id,
                    OrderId = order.Id,
                    Amount = amount,
                    NetUnitPrice = product.NetUnitPrice,
                    TaxRate = product.Category.TaxRate
                };

                // OrderLine speichern
                db.OrderLines.Add(newOrderLine);
                await db.SaveChangesAsync();

                UpdateOrderTotalPrice(order);
            }
        }

        // Den TotalPrice in Order berechnen und speichern
        private async void UpdateOrderTotalPrice(Order order)
        {
            decimal totalPrice = 0;

            using (var db = new LapWebshopContext())
            {
                // Alle Produkte im Einkaufswagen holen die zu einer nicht abgeschlossenen Bestellung gehören
                var productsInShoppingCart = db.OrderLines.Where(x => x.OrderId == order.Id && order.DateOrdered == null);

                foreach (var item in productsInShoppingCart)
                {
                    decimal itemBruttoPrice = item.NetUnitPrice / 100 * (100 + item.TaxRate);
                    decimal allItems = item.Amount * itemBruttoPrice;

                    totalPrice += allItems;
                }

                order.PriceTotal = totalPrice;

                db.Update(order);
                await db.SaveChangesAsync();
            }
        }
    }
}
