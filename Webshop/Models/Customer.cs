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
        //[RegularExpression("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01 -\\x08\\x0b\\x0c\\x0e -\\x1f\\x21\\x23 -\\x5b\\x5d -\\x7f] |\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])")]
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
