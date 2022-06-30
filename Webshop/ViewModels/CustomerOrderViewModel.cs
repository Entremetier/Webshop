using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.ViewModels
{
    public class CustomerOrderViewModel
    {
        public OrderLine orderline { get; set; }
        public int ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Manufracturer { get; set; }
        public decimal? BruttoPrice { get; set; }
        public decimal? RowPrice { get; set; }
        public string ImagePath { get; set; }
        public List<SelectListItem> selectList { get; set; }
    }
}
