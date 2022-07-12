using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.ViewModels
{
    public class FinishedOrderViewModel
    {
        public decimal BruttoPrice { get; set; }
        public decimal BruttoRowPrice { get; set; }
        public OrderLine OrderLine { get; set; }
    }
}
