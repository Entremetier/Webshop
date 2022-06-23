using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class UserService
    {
        public Customer GetCurrentUser(string email)
        {
            using (var db = new LapWebshopContext())
            {
                var customer = db.Customers.Where(e => e.Email == email)
                        .FirstOrDefault();

                return customer;
            }
        }
    }
}
