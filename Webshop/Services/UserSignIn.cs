using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public class UserSignIn
    {
        public ClaimsIdentity GetClaimsIdentity(string email, int userId)
        {
            // Claims erstellen um im späteren Verlauf den Benutzer zu identifizieren
            var emailClaim = new Claim(ClaimTypes.Email, email);
            var idClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

            var claims = new List<Claim>() { emailClaim, idClaim };

            // Die Identität des Users zurückschicken
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return claimsIdentity;
        }

        public ClaimsPrincipal GetClaimsPrincipal(ClaimsIdentity claimsIdentity)
        {
            //Die Claims wandern in eine Identity, welche wir für den Principal (den Rechteinhaber) benötigen
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
