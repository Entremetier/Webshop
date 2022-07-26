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
using System.Diagnostics;

namespace Webshop.Controllers
{
    public class ViewToPdfController : Controller
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly PdfService _pdfService;
        private readonly OrderLineService _orderLineService;
        private readonly PaymentService _paymentService;

        public ViewToPdfController(
            UserService userService,
            OrderService orderService,
            PdfService pdfService,
            OrderLineService orderLineService,
            PaymentService paymentService)
        {
            _userService = userService;
            _orderService = orderService;
            _pdfService = pdfService;
            _orderLineService = orderLineService;
            _paymentService = paymentService;
        }

        [Authorize]
        public async Task<IActionResult> Checkout(string firstName, string lastName, string street, string zip, string city, string payment, decimal cardnumber, string cardOwner, int secureNumber)
        {
            if (payment == "1")
            {
                bool isCardValid = _paymentService.CreditCardValidation(cardnumber);
                if (isCardValid == false || cardnumber == 0 || cardOwner == null || secureNumber == 0)
                {
                    TempData["CreditCardWarning"] = "Gültige Kreditkarte angeben!";
                    return RedirectToAction("Order", "Order");
                }
            }

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

            await _orderService.SetOrder(order, firstName, lastName, street, zip, city, payment, cardnumber, cardOwner, secureNumber);

            CustomerAndOrderIDViewModel customerOrderVM = new CustomerAndOrderIDViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                OrderId = order.Id
            };

            // Letzte abgeschlossenes Bestellung holen
            var finishedOrder = await _orderService.GetLastFinishedOrder(customer);

            var completeOrder = await _pdfService.GetPdfData(finishedOrder);

            List<OrderLine> orderLines = await _orderLineService.GetOrderLinesOfOrderWithProductAndManufacturer(completeOrder);

            decimal fullNettoPrice = _orderService.GetFullNettoPriceOfOrderLines(orderLines);
            List<FinishedOrderViewModel> finishedOrderVMList = await _orderLineService.GetFinishedOrderVMList(orderLines);

            FinishedOrderContainerViewModel finishedOrderContainerVM = new FinishedOrderContainerViewModel();
            finishedOrderContainerVM.Customer = customer;
            finishedOrderContainerVM.Order = order;
            finishedOrderContainerVM.FinishedOrderViewModels = finishedOrderVMList;
            finishedOrderContainerVM.FullNettoPrice = fullNettoPrice;
            finishedOrderContainerVM.Taxes = order.PriceTotal - fullNettoPrice;

            //Task.Run(async () =>
            //{
            var viewAsPdf = UserCheck(finishedOrderContainerVM);
            byte[] pdfAsByteArray = await viewAsPdf.BuildFile(ControllerContext);
            Stream fileStream = new MemoryStream(pdfAsByteArray);
            MailService.SendMail(customer.FirstName, customer.LastName, customer.Email, fileStream);
            //});

            return RedirectToAction("UserCheckout", customerOrderVM);
        }

        [Authorize]
        public IActionResult UserCheckout(CustomerAndOrderIDViewModel customerAndOrderVM)
        {
            return View(customerAndOrderVM);
        }

        [Authorize]
        private static ViewAsPdf UserCheck(FinishedOrderContainerViewModel finishedOrderVM)
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
