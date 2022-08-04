using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class ProductsWithAmount
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public DateTime DateOrdered { get; set; }
    }
}
