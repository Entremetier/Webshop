using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class PdfService
    {
        private readonly OrderService _orderService;

        public PdfService(OrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Order> GetPdfData(Order order)
        {
            using (var db = new LapWebshopContext())
            {
                return await db.Orders.Where(o => o.DateOrdered == order.DateOrdered)
                    .Include(c => c.Customer)
                    .Include(ol => ol.OrderLines)
                    .ThenInclude(x => x.Product)
                    .ThenInclude(m => m.Manufacturer)
                    .OrderBy(o => o)
                    .LastOrDefaultAsync();
            }
        }
    }
}
