﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;
using Microsoft.EntityFrameworkCore;
using Webshop.ViewModels;

namespace Webshop.Services
{
    public class OrderLineService
    {
        private readonly OrderService _orderService;
        private readonly UserService _userService;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public OrderLineService
            (OrderService orderService,
            UserService userService,
            ProductService productService,
            CategoryService categoryService)
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
            _categoryService = categoryService;
        }

        // In den Details die ausgesuchte Menge in den Warenkorb legen
        public async Task<string> AddProductToShoppingCart(Product product, Order order, int amount)
        {
            using (var db = new LapWebshopContext())
            {
                // Kontrollieren ob ein Produkt schon im Warenkorb, des angemeldeten users, ist
                var productAlreadyInCart = await db.OrderLines.Where(x => x.ProductId == product.Id && order.Id == x.OrderId && order.DateOrdered == null)
                    .FirstOrDefaultAsync();

                if (productAlreadyInCart != null)
                {
                    if (productAlreadyInCart.Amount >= MaxItemsInCart.MaxItemsInShoppingCart)
                    {
                        return "false";
                    }
                    await IncrementAmountOfProduct(productAlreadyInCart, order, amount);
                }
                else
                {
                    await CreateNewOrderLine(product, order, amount);
                }
                return "true";
            }
        }

        private async Task IncrementAmountOfProduct(OrderLine productAlreadyInCart, Order order, int amount)
        {
            using (var db = new LapWebshopContext())
            {
                // Menge des vorhandenen Produktes im Warenkorb erhöhen
                productAlreadyInCart.Amount += amount;

                db.Update(productAlreadyInCart);
                await db.SaveChangesAsync();

                await UpdateOrderTotalPrice(order);
            }
        }

        // Im Warenkorb die Menge um eins verringern
        public async Task DecrementAmountOfProductByOne(int productId, int newAmount, string email)
        {
            var customer = await _userService.GetCurrentUser(email);
            var order = await _orderService.GetOrder(customer);

            using (var db = new LapWebshopContext())
            {
                var orderLine = await db.OrderLines
                    .Where(o => o.ProductId == productId && order.DateOrdered == null && o.OrderId == order.Id)
                    .FirstOrDefaultAsync();

                orderLine.Amount = newAmount;

                db.Update(orderLine);
                await db.SaveChangesAsync();

                await UpdateOrderTotalPrice(order);
            }
        }

        // Im Warenkorb die Menge um eins erhöhen
        public async Task IncrementAmountOfProductByOne(int productId, int newAmount, string email)
        {
            var customer = await _userService.GetCurrentUser(email);
            var order = await _orderService.GetOrder(customer);

            using (var db = new LapWebshopContext())
            {
                var orderLine = await db.OrderLines
                    .Where(o => o.ProductId == productId && order.DateOrdered == null && o.OrderId == order.Id)
                    .FirstOrDefaultAsync();

                orderLine.Amount = newAmount;

                // Das aktuelle Produkt holen
                var product = await _productService.GetProductWithManufacturer(productId);

                // Die Menge kann nicht größer sein als der Lagerbestand, wenn doch auf Menge im Lager setzen
                if (orderLine.Amount > product.Lagerstand.Value)
                {
                    orderLine.Amount = product.Lagerstand.Value;
                }

                db.Update(orderLine);
                await db.SaveChangesAsync();

                await UpdateOrderTotalPrice(order);
            }
        }

        public async Task DeleteOrderLine(string email, int productId)
        {
            var customer = await _userService.GetCurrentUser(email);
            var order = await _orderService.GetOrder(customer);

            using (var db = new LapWebshopContext())
            {
                // Die gesuchte OrderLine aus DB holen
                OrderLine orderLine = await db.OrderLines
                    .Where(o => o.ProductId == productId && order.DateOrdered == null && o.OrderId == order.Id)
                    .FirstOrDefaultAsync();

                // OrderLine löschen
                db.Remove(orderLine);
                await db.SaveChangesAsync();

                await UpdateOrderTotalPrice(order);
            }
        }

        private async Task CreateNewOrderLine(Product product, Order order, int amount)
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

                await UpdateOrderTotalPrice(order);
            }
        }

        // Den TotalPrice in Order berechnen und speichern
        private async Task UpdateOrderTotalPrice(Order order)
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

        public async Task<List<OrderLine>> GetOrderLinesOfOrder(Order order)
        {
            using (var db = new LapWebshopContext())
            {
                return await db.OrderLines.Where(x => x.OrderId == order.Id).ToListAsync();
            }
        }

        public async Task<List<OrderLine>> GetOrderLinesOfOrderWithProductAndManufacturer(Order order)
        {
            using (var db = new LapWebshopContext())
            {
                return await db.OrderLines
                    .Include(p => p.Product)
                    .ThenInclude(m => m.Manufacturer)
                    .Where(x => x.OrderId == order.Id)
                    .ToListAsync();
            }
        }


        //public async Task<int> GetAmountOfProductInCard(Customer customer, Product product)
        //{
        //    int amountInCart = 0;
        //    var order = await _orderService.GetOrder(customer);
        //    var orderLines = await GetOrderLinesOfOrder(order);

        //    foreach (var orderLine in orderLines)
        //    {
        //        if (orderLine.Id == product.Id)
        //        {
        //            amountInCart = orderLine.Amount;
        //        }
        //    }

        //    return amountInCart;
        //}

        public List<SelectListItem> FillSelectList(int amount)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            for (int i = amount; i > 0; i--)
            {
                listItems.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
            return listItems;
        }

        public async Task<List<FinishedOrderViewModel>> GetFinishedOrderVMList(List<OrderLine> orderLines)
        {
            FinishedOrderViewModel finishedOrderVM;
            List<FinishedOrderViewModel> finishedOrderVMList = new List<FinishedOrderViewModel>();

            var categoryAndTaxRate = await _categoryService.GetAllCategoriesAndTaxRates();

            foreach (var item in orderLines)
            {
                finishedOrderVM = new FinishedOrderViewModel
                {
                    BruttoPrice = _productService.CalcPrice(item.Product, categoryAndTaxRate),
                    BruttoRowPrice = item.Amount * _productService.CalcPrice(item.Product, categoryAndTaxRate),
                    OrderLine = item
                };
                finishedOrderVMList.Add(finishedOrderVM);
            }
            return finishedOrderVMList;
        }
    }
}
