using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    public class ShoppingCartController : Controller
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly OrderLineService _orderLineService;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ShoppingCartController(
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

        [HttpPost]
        public async Task<IActionResult> AddToShoppingCart(int amount, int id)
        {
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt, user ist nicht eingeloggt, zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            // Customer aus der DB holen
            var customer = await _userService.GetCurrentUser(email);

            // Das gewählte Produkt aus DB holen
            var product = await _productService.GetProductWithManufacturerAndCategory(id);

            // Wenn es das Produkt nicht gibt
            if (product == null)
            {
                return RedirectToAction("Shop", "Home");
            }

            // Die offene Order des Users heraussuchen (DateOrdered == null)
            var order = await _orderService.GetOrder(customer);

            // Im Warenkorb schauen ob es das Produkt mit der gesuchten ProduktId schon gibt
            _orderLineService.AddProductToShoppingCart(product, order, amount);

            // Die Details Seite mit dem Produkt und geändertem DDL laden (wenn aus der Details Seite aufgerufen
            // wurde, direkt aus Shop wird die Seite nicht neu geladen (jQuery im Shop.cshtml))
            return RedirectToAction("Details", "Product", product);
        }

        public async Task<IActionResult> Cart()
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
            var categoryAndTaxRate = _categoryService.GetAllCategoriesAndTaxRates();
            List<OrderLine> orderLines =  _orderLineService.GetOrderLinesOfOrder(order);

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
                        // Bruttopreis auf zwei Nachkommastellen runden
                        BruttoPrice = Math.Round(_productService.CalcPrice(product, categoryAndTaxRate), 2),
                        ImagePath = product.ImagePath,
                        Orderline = item,
                        RowPrice = Math.Round(item.Amount * _productService.CalcPrice(product, categoryAndTaxRate), 2),
                        SelectList = _orderLineService.FillSelectList(item.Amount)
                    });
                }

                ViewBag.Order = order;
                ViewBag.Customer = customer;
                return View(viewModelList);
            }
        }

        public IActionResult DeleteFromCart(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Cart", "ShoppingCart");
            }
            else
            {
                string email = User.FindFirstValue(ClaimTypes.Email);

                if (email == null)
                {
                    return RedirectToAction("Login", "Customer");
                }

                _orderLineService.DeleteOrderLine(email, id.Value);

                return RedirectToAction("Cart", "ShoppingCart");
            }
        }

        public IActionResult DecrementValue(int? id, int amountInCart)
        {
            string email = User.FindFirstValue(ClaimTypes.Email);

            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            if (!id.HasValue)
            {
                return RedirectToAction("Cart", "ShoppingCart");
            }
            else
            {
                _orderLineService.DecrementAmountOfProduct(id.Value, amountInCart, email);
                return RedirectToAction("Cart", "ShoppingCart");
            }
        }
    }
}
