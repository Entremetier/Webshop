﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Webshop.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderLines = new HashSet<OrderLine>();
            OrderPayments = new HashSet<OrderPayment>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal PriceTotal { get; set; }
        public DateTime? DateOrdered { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
        public virtual ICollection<OrderPayment> OrderPayments { get; set; }
    }
}
