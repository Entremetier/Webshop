﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;
using Webshop.ViewModels;

namespace Webshop.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly OrderLineService _orderLineService;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public OrderController(
            UserService userService,
            OrderService orderService,
            OrderLineService orderLineService,
            ProductService productService,
            CategoryService categoryService)
        {
            _userService = userService;
            _orderService = orderService;
            _orderLineService = orderLineService;
            _productService = productService;
            _categoryService = categoryService;
        }

        [Authorize]
        public async Task<IActionResult> Order()
        {
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt, user ist nicht eingeloggt, zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            var customer = await _userService.GetCurrentUser(email);
            var order = await _orderService.GetOrder(customer);
            var categoryAndTaxRate = await _categoryService.GetAllCategoriesAndTaxRates();
            List<OrderLine> orderLines = await _orderLineService.GetOrderLinesOfOrder(order);

            // Wenn der Gesamtpreis der Waren im Warenkorb kleiner gleich 0 ist in den Shop umleiten
            if (order.PriceTotal <= 0)
            {
                return RedirectToAction("Shop", "Home");
            }

            if (customer == null || order == null || categoryAndTaxRate == null || orderLines == null)
            {
                return RedirectToAction("Shop", "Home");
            }
            else
            {
                List<CustomerOrderViewModel> viewModelList = new List<CustomerOrderViewModel>();
                foreach (var item in orderLines)
                {

                    var product = await _productService.GetProductWithManufacturer(item.ProductId);
                    viewModelList.Add(new CustomerOrderViewModel
                    {
                        ProductNumber = item.ProductId,
                        ProductName = product.ProductName,
                        Manufacturer = product.Manufacturer.Name,
                        BruttoPrice = _productService.CalcPrice(product, categoryAndTaxRate),
                        ImagePath = product.ImagePath,
                        Orderline = item,
                        RowPrice = item.Amount * _productService.CalcPrice(product, categoryAndTaxRate),
                        SelectList = _orderLineService.FillSelectList(item.Amount)
                    });
                }

                if (viewModelList.Count <= 0)
                {
                    TempData["NoItems"] = "Es befinden sich keine Produkte im Warenkorb!";
                }

                ViewBag.Order = order;
                ViewBag.Customer = customer;
                return View(viewModelList);
            }
        }

        [Authorize]
        public async Task<IActionResult> Checkout(string firstName, string lastName, string street, string zip, string city)
        {
            if (firstName == null || lastName == null || street == null || zip == null || city == null)
            {
                TempData["Warning"] = "Fehlende Daten bei der Lieferadresse!";
                return RedirectToAction("Order", "Order");
            }
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt, user ist nicht eingeloggt, zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            var customer = await _userService.GetCurrentUser(email);
            var order = await _orderService.GetOrder(customer);

            if (order.PriceTotal <= 0)
            {
                return RedirectToAction("Shop", "Home");
            }

            await _orderService.MakeOrder(order, firstName, lastName, street, zip, city);

            ViewBag.Order = order;
            ViewBag.Customer = customer;
            return View();
        }
    }
}
