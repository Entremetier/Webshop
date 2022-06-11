using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Webshop.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Anrede angeben")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vornamen angeben")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nachnamen angeben")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-Mail angeben")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Straße und Hausnummer angeben")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Postleizahl angeben")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "Bitte eine gültige österreichische Postleitzahl angeben")]
        public int Zip { get; set; }

        [Required(ErrorMessage = "Stadt angeben")]
        public string City { get; set; }

        public string PwHash { get; set; }

        public string Salt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
