using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;

namespace Webshop.Services
{
    public class GetCurrentUser : Controller
    {
        //public string GetUserEmail(ClaimsPrincipal principal)
        //{
        //    // Die E-Mail des angemeldeten User mittels E-Mail-Claim bekommen
        //    string email = User.FindFirstValue(ClaimTypes.Email);
        //    return email;
        //}
    }
}
