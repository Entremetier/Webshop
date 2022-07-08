﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.ViewModels
{
    public class FinishedOrderViewModel
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public decimal FullNettoPrice { get; set; }
        public decimal BruttoPrice { get; set; }
        public decimal RowPrice { get; set; }
        public decimal Taxes { get; set; }
        public List<OrderLine> OrderLines { get; set; }
    }
}
