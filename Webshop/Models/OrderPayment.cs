﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Webshop.Models
{
    public partial class OrderPayment
    {
        public int Id { get; set; }
        public string CreditCardNumber { get; set; }
        public string SecureCode { get; set; }
        public string CardOwnerName { get; set; }
        public int? OrderId { get; set; }
        public int? PaymentId { get; set; }

        public virtual Order Order { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
