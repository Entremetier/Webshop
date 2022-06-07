using System;
using System.Collections.Generic;

#nullable disable

namespace Webshop.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal NetUnitPrice { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int ManufacturerId { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}
