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
        //public async Task SignUserInAsync(string email, int userId)
        //{
        //    //Zunächst erstellen wir ein paar Claims - das hilft uns in weiterer Folge den Benutzer zu identifizieren
        //    var emailClaim = new Claim(ClaimTypes.Email, email);
        //    var idClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

        //    var claims = new List<Claim>() { emailClaim, idClaim };

        //    //Die Claims wandern in eine Identity, welche wir für den Principal (den Rechteinhaber) benötigen
        //    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //    //Und diesen Rechteinhaber können wir als angemeldet markieren
        //    //Im Hintergrund wird das User-Objekt befüllt
        //    //Und dem Benutzer ein Cookie mitgeliedert

        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        //}

        public ClaimsIdentity GetClaimsIdentity(string email, int userId)
        {
            // Claims erstellen um im späteren Verlauf den Benutzer zu identifizieren
            var emailClaim = new Claim(ClaimTypes.Email, email);
            var idClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

            var claims = new List<Claim>() { emailClaim, idClaim };

            // Die Identität des Users zurückschicken
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return claimsIdentity;
        }

        public ClaimsPrincipal GetClaimsPrincipal(ClaimsIdentity claimsIdentity)
        {
            //Die Claims wandern in eine Identity, welche wir für den Principal (den Rechteinhaber) benötigen
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
