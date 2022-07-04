using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webshop.Services;
using Rotativa.AspNetCore.Options;

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
        public async Task<IActionResult> UserCheck()
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

            string customPdf =
                $"--footer-left \" {DateTime.Now.Date.ToString("dd/MM/yyyy")}\" " +
                $"--footer-right \"Seite[page] von [toPage]\" " +
                $"--footer-font-size\"8\" ";

            //string viewName = "UserCheck";

            // Letzte abgeschlossenes Bestellung holen
            var order = await _orderService.GetLastFinishedOrder(customer);

            var completeOrder = await _pdfService.GetPdfData(order);

            return new ViewAsPdf(completeOrder)
            {
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                PageMargins = new Margins(15, 20, 15, 25),
                CustomSwitches = customPdf // Konfiguration der PDF-Seite einbinden
            };
        }
    }
}
