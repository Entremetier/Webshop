using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly UserService _userService;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly OrderLineService _orderLineService;

        public ShoppingCartController(
            UserService userService,
            ProductService productService,
            OrderService orderService,
            OrderLineService orderLineService)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _orderLineService = orderLineService;
        }


        public IActionResult AddToShoppingCart(string amount, int id)
        {
            int.TryParse(amount, out int amountInt);

            // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
            string email = User.FindFirstValue(ClaimTypes.Email);

            // Wenn es keine Email gibt zurück zum Login schicken
            if (email == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            // Customer aus der DB holen
            var customer = _userService.GetCurrentUser(email);

            // Das gewählte Produkt aus DB holen
            var product = _productService.GetProduct(id);

            // Wenn es das Produkt nicht gibt
            if (product == null)
            {
                return NotFound();
            }

            // Die offene Bestellung des Users heraussuchen (DateOrdered == null)
            var order = _orderService.GetOrder(customer);

            // Wenn es keine offene Order gibt
            if (order == null)
            {
                // Eine neue Order erstellen, da es Produkt und Customer gibt
                _orderService.CreateOrder(customer);

                // Die neu erstellte, offene Bestellung des Users heraussuchen
                order = _orderService.GetOrder(customer);
            }

            // Im Warenkorb schauen ob es das Produkt mit der gesuchten ProduktId schon gibt
            _orderLineService.GetProductIfInShoppingCart(product, order, amountInt);

            // Die Seite nicht neu laden
            return RedirectToAction("Shop", "Home");
        }
    }
}
