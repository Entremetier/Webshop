using System;
using System.Collections.Generic;

#nullable disable

namespace Webshop.Models
{
    public partial class Payment
    {
        public Payment()
        {
            OrderPayments = new HashSet<OrderPayment>();
        }

        public int Id { get; set; }
        public string PaymentName { get; set; }

        public virtual ICollection<OrderPayment> OrderPayments { get; set; }
    }
}
