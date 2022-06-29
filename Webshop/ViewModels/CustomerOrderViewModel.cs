using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.ViewModels
{
    public class CustomerOrderViewModel
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public List<OrderLine> OrderLine { get; set; }
    }
}
