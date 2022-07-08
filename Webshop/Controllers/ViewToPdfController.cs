using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webshop.Services;
using Rotativa.AspNetCore.Options;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Webshop.Models;
using Webshop.ViewModels;

namespace Webshop.Controllers
{
    public class ViewToPdfController : Controller
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly PdfService _pdfService;
        private readonly CategoryService _categoryService;
        private readonly OrderLineService _orderLineService;
        private readonly ProductService _productService;

        public ViewToPdfController(UserService userService, OrderService orderService, PdfService pdfService, CategoryService categoryService,
            OrderLineService orderLineService, ProductService productService)
        {
            _userService = userService;
            _orderService = orderService;
            _pdfService = pdfService;
            _categoryService = categoryService;
            _orderLineService = orderLineService;
            _productService = productService;
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

            await _orderService.SetOrder(order, firstName, lastName, street, zip, city);

            CustomerAndOrderIDViewModel customerOrderVM = new CustomerAndOrderIDViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                OrderId = order.Id
            };

            // Letzte abgeschlossenes Bestellung holen
            var finishedOrder = await _orderService.GetLastFinishedOrder(customer);

            var completeOrder = await _pdfService.GetPdfData(finishedOrder);


            var categoryAndTaxRate = await _categoryService.GetAllCategoriesAndTaxRates();
            List<OrderLine> orderLines = await _orderLineService.GetOrderLinesOfOrderWithProductAndManufacturer(finishedOrder);

            decimal fullNettoPrice = 0;
            foreach (var item in orderLines)
            {
                fullNettoPrice += item.Amount * item.NetUnitPrice;
            }

            FinishedOrderViewModel finishedOrderVM = new FinishedOrderViewModel();
            foreach (var item in orderLines)
            {
                finishedOrderVM.Customer = customer;
                finishedOrderVM.Order = finishedOrder;
                finishedOrderVM.FullNettoPrice = fullNettoPrice;
                finishedOrderVM.BruttoPrice = _productService.CalcPrice(item.Product, categoryAndTaxRate);
                finishedOrderVM.RowPrice = item.Amount * _productService.CalcPrice(item.Product, categoryAndTaxRate);
                finishedOrderVM.Taxes = order.PriceTotal - fullNettoPrice;
                finishedOrderVM.OrderLines = await _orderLineService.GetOrderLinesOfOrderWithProductAndManufacturer(completeOrder);
            }

            var viewAsPdf = UserCheck(finishedOrderVM);
            byte[] pdfAsByteArray = await viewAsPdf.BuildFile(ControllerContext);
            Stream fileStream = new MemoryStream(pdfAsByteArray);
            MailService.SendMail(customer.FirstName, customer.LastName, customer.Email, fileStream);

            return RedirectToAction("UserCheckout", customerOrderVM);
        }

        [Authorize]
        public IActionResult UserCheckout(CustomerAndOrderIDViewModel customerAndOrderVM)
        {
            return View(customerAndOrderVM);
        }

        private static ViewAsPdf UserCheck(FinishedOrderViewModel finishedOrderVM)
        {
            string viewName = "UserCheck";

            return new ViewAsPdf(viewName, finishedOrderVM)
            {
                FileName = "Rechnung",
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                PageMargins = new Margins(15, 20, 15, 25),
            };
        }
    }
}
