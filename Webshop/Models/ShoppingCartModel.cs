using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class ShoppingCartModel
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string ProductName { get; set; }
        public string ManufacturerName { get; set; }
        public decimal NetUnitPrice { get; set; }
        public decimal BruttoUnitPrice { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
    }
}
