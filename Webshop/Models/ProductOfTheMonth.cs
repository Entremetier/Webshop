using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class ProductOfTheMonth
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public DateTime DateOrdered { get; set; }
    }
}
