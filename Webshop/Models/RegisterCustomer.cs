using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class RegisterCustomer
    {
        public RegisterCustomer()
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
        // Nur minimale Validierung der E-Mail, besser wäre es eine Mail an die Adresse zu schicken wo der User einen bestätigungslink drücken muss.
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Straße und Hausnummer angeben")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Postleizahl angeben")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "Bitte eine gültige österreichische Postleitzahl angeben")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Stadt angeben")]
        public string City { get; set; }
        public byte[] PwHash { get; set; }

        [Required(ErrorMessage = "Passwort muss aus mindestens 8 Zeichen, einer Zahl, einem Groß- und einem Kleinbuchstaben bestehen")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage ="Passwort muss aus mindestens 8 Zeichen, einer Zahl, " +
            "einem Groß-, einem Kleinbuchstaben und einem Sonderzeichen bestehen")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Passwort wiederholen")]
        [Compare("Password", ErrorMessage = "Passwörter müssen übereinstimmen")]
        public string ConfirmPassword { get; set; }

        public byte[] Salt { get; set; }


        public virtual ICollection<Order> Orders { get; set; }
    }
}
