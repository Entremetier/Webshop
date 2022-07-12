using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.ViewModels
{
    public class AnotherViewModel
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public decimal FullNettoPrice { get; set; }
        public decimal Taxes { get; set; }

        public List<FinishedOrderViewModel> FinishedOrderViewModels { get; set; }
    }
}
