using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class ProductWithListOfChoosenProduct
    {
        public ProductWithListOfChoosenProduct()
        {
            OrderLines = new HashSet<OrderLine>();
            ProduktAufruves = new HashSet<ProduktAufrufe>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal NetUnitPrice { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int ManufacturerId { get; set; }
        public int CategoryId { get; set; }
        public List<Product> ProductList { get; set; }
        public virtual Category Category { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
        public virtual ICollection<ProduktAufrufe> ProduktAufruves { get; set; }
    }
}
