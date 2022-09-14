using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class UserService
    {
        private readonly LapWebshopContext _context;

        public UserService(LapWebshopContext context)
        {
            _context = context;
        }

        public async Task RegisterUserAsync(RegisterCustomer customer, string password)
        {
            // 1. Salt erzeugen

            var saltBytes = new byte[256 / 8];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);


            //Password Hashen
            var hash = HashUtf8PasswordWithSha256AndSalt(password, saltBytes);

            // 2. Benutzerdaten inkl. Hash und Salt in DB speichern 
            var newCustomer = new Customer
            {
                // Werte des neuen Users ohne führende und endende Leerzeichen übernehmen
                Email = customer.Email.Trim(),
                Title = customer.Title.Trim(),
                FirstName = customer.FirstName.Trim(),
                LastName = customer.LastName.Trim(),
                City = customer.City.Trim(),
                Street = customer.Street.Trim(),
                Zip = customer.Zip.Trim(),
                PwHash = hash,
                Salt = saltBytes
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer> CanUserLogInAsync(string email, string password)
        {
            // 1. Benutzerdaten laden
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);

            //      Falls nicht geladen --> darf sich nicht anmelden --> return null
            if (customer is null) return null;

            //      Falls geladen werden konnte:
            // 2. Login-Passwort mit gespeichertem Salt hashen
            var hash = HashUtf8PasswordWithSha256AndSalt(password, customer.Salt);

            // 3. Hashes verlgeichen
            //      Falls gleich --> return Customer
            if (hash.SequenceEqual(customer.PwHash)) return customer;
            //      Falls nicht gleich --> darf sich nicht anmelden --> return null
            else return null;
        }

        private byte[] HashUtf8PasswordWithSha256AndSalt(string password, byte[] salt)
        {
            // 1. String-Passwort in Byte-Array umwandeln
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            // 2. Salt an Passwort hängen
            var saltedPasswordBytes = passwordBytes.Concat(salt).ToArray();

            // 3. Gesaltetes Passwort hashen
            var hasher = SHA256.Create();

            var hashedPasswordBytes = hasher.ComputeHash(saltedPasswordBytes);

            return hashedPasswordBytes;
        }

        public ClaimsIdentity GetClaimsIdentity(string email, Customer user)
        {
            // Claims erstellen um im späteren Verlauf den Benutzer zu identifizieren
            var emailClaim = new Claim(ClaimTypes.Email, email);
            var idClaim = new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());

            // Wenn in der Datenbank noch User sind die keine Rolle haben werden sie auf Customer gesetzt
            if (user.Role == null)
            {
                user.Role = "Customer";
            }

            var roleClaim = new Claim(ClaimTypes.Role, user.Role);

            var claims = new List<Claim>() { emailClaim, idClaim, roleClaim };

            // Die Identität des Users zurückschicken
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return claimsIdentity;
        }

        public ClaimsPrincipal GetClaimsPrincipal(ClaimsIdentity claimsIdentity)
        {
            //Die Claims wandern in eine Identity, welche wir für den Principal (den Rechteinhaber) benötigen
            return new ClaimsPrincipal(claimsIdentity);
        }

        public async Task<Customer> GetCurrentUser(string email)
        {
            using (var db = new LapWebshopContext())
            {
                var customer = await db.Customers.Where(e => e.Email == email)
                        .FirstOrDefaultAsync();

                return customer;
            }
        }


    }
}
