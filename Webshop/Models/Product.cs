using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Produktname angeben")]
        [MaxLength(10, ErrorMessage = "Maximal 100 Zeichen")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "NettoPreis/€ angeben")]
        public decimal NetUnitPrice { get; set; }

        [Required(ErrorMessage = "Bildpfad angeben")]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Produktbeschreibung angeben")]
        public string Description { get; set; }

        public int ManufacturerId { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}
