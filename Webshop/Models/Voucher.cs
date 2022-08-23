using System;
using System.Collections.Generic;

#nullable disable

namespace Webshop.Models
{
    public partial class Voucher
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string VoucherCode { get; set; }
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
