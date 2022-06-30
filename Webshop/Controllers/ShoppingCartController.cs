using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
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

        public ShoppingCartController(
            UserService userService,
            OrderService orderService,
            OrderLineService orderLineService,
            ProductService productService)
        {
            _userService = userService;
            _orderService = orderService;
            _orderLineService = orderLineService;
            _productService = productService;
        }

        [HttpPost]
        public IActionResult AddToShoppingCart(string amount, int id)
        {
            // Parsen weil die Menge von der DDL als string kommt
            int.TryParse(amount, out int amountInt);

            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt, user ist nicht eingeloggt, zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            // Customer aus der DB holen
            var customer = _userService.GetCurrentUser(email);

            // Das gewählte Produkt aus DB holen
            var product = _productService.GetProductWithManufacturerAndCategory(id);

            // Wenn es das Produkt nicht gibt
            if (product == null)
            {
                return RedirectToAction("Shop", "Home");
            }

            // Die offene Order des Users heraussuchen (DateOrdered == null)
            var order = _orderService.GetOrder(customer);

            // Im Warenkorb schauen ob es das Produkt mit der gesuchten ProduktId schon gibt
            _orderLineService.AddProductToShoppingCart(product, order, amountInt);

            // Die Details Seite mit dem Produkt und geändertem DDL laden (wenn aus der Details Seite aufgerufen
            // wurde, direkt aus Shop wird die Seite nicht neu geladen (jQuery im Shop.cshtml))
            return RedirectToAction("Details", "Product", product);
        }

        public IActionResult Cart()
        {
            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt, user ist nicht eingeloggt, zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            var customer = _userService.GetCurrentUser(email);
            var order = _orderService.GetOrder(customer);
            List<OrderLine> orderLines = _orderLineService.GetOrderLinesOfOrder(order);

            if (customer == null || order == null)
            {
                return RedirectToAction("Shop", "Home");
            }
            else
            {
                List<CustomerOrderViewModel> viewModelList = new List<CustomerOrderViewModel>();
                foreach (var item in orderLines)
                {
                    var product = _productService.GetProductWithManufacturer(item.ProductId);
                    viewModelList.Add(new CustomerOrderViewModel
                    {
                        ProductNumber = item.ProductId,
                        ProductName = product.ProductName,
                        Manufracturer = product.Manufacturer.Name,
                        // TODO: Berechnung einfügen
                        BruttoPrice = (item.NetUnitPrice / 100) * 120,
                        ImagePath = product.ImagePath,
                        orderline = item
                    });
                }

                return View(viewModelList);
            }
        }
    }
}
