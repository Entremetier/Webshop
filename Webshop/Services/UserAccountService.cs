using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class UserAccountService
    {
        private readonly LapWebshopContext _context;

        public UserAccountService(LapWebshopContext context)
        {
            _context = context;
        }

        public async Task RegisterUserAsync(CustomerLogin customer, string password)
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
                Email = customer.Email,
                Title = customer.Title,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                City = customer.City,
                Street = customer.Street,
                Zip = customer.Zip,
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
    }
}
