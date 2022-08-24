using System;
using System.Collections.Generic;

#nullable disable

namespace Webshop.Models
{
    public partial class ProduktAufrufe
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Calls { get; set; }

        public virtual Product Product { get; set; }
    }
}
