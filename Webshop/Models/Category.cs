using System;
using System.Collections.Generic;

#nullable disable

namespace Webshop.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TaxRate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
