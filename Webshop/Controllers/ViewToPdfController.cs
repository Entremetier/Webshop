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

        public ViewToPdfController(UserService userService, OrderService orderService, PdfService pdfService)
        {
            _userService = userService;
            _orderService = orderService;
            _pdfService = pdfService;
        }

        //[Authorize]
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

            //if (order.PriceTotal <= 0)
            //{
            //    return RedirectToAction("Shop", "Home");
            //}

            await _orderService.MakeOrder(order, firstName, lastName, street, zip, city);

            CustomerAndOrderIDViewModel customerOrderVM = new CustomerAndOrderIDViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                OrderId = order.Id
            };
            // Letzte abgeschlossenes Bestellung holen
            //var order = await _orderService.GetLastFinishedOrder(customer);

            var completeOrder = await _pdfService.GetPdfData(order);

            var viewAsPdf = UserCheck(completeOrder);
            byte[] pdfAsByteArray = await viewAsPdf.BuildFile(ControllerContext);
            //string fullPath = @"~\Pdf\" + pdf.FileName;
            Stream fileStream = new MemoryStream(pdfAsByteArray);
            MailService.SendMail(customer.Email, fileStream);

            return RedirectToAction("Checkout", customerOrderVM);
        }
        private static ViewAsPdf UserCheck(Order completeOrder)
        {
            string viewName = "UserCheck";
            return new ViewAsPdf(viewName, completeOrder)
            {
                FileName = "Rechnung",
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                PageMargins = new Margins(15, 20, 15, 25),
                //CustomSwitches = customPdf // Konfiguration der PDF-Seite einbinden
            };
        }
    }
}
