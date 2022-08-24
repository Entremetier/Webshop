using System;
using System.Collections.Generic;

#nullable disable

namespace Webshop.Models
{
    public partial class Bewertungen
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int? Points { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
