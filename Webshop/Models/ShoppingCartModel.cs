using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class ShoppingCartModel
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public decimal NetUnitPrice { get; set; }
        public decimal TaxRate { get; set; }
    }
}
