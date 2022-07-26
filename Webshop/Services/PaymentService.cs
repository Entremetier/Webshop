using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class PaymentService
    {
        public List<SelectListItem> GetListOfPayments()
        {
            List<SelectListItem> methodsOfPayments = new List<SelectListItem>();

            using (var db = new LapWebshopContext())
            {
                var payments = db.Payments.Select(p => p.PaymentName).ToList();

                for (int i = 0; i < payments.Count(); i++)
                {
                    methodsOfPayments.Add(new SelectListItem { Value = i.ToString(), Text = payments[i]});
                }
            }

            return methodsOfPayments;
        }

        public bool CreditCardValidation(decimal cardNumber)
        {
            // cardnumber zu einem Array machen
            int[] cardNumberArray = cardNumber.ToString().Select(num => Convert.ToInt32(num)).ToArray();

            int nDigits = cardNumberArray.Length;

            int nSum = 0;
            bool isSecond = false;
            for (int i = nDigits - 1; i >= 0; i--)
            {

                int d = cardNumberArray[i] - '0';

                if (isSecond == true)
                    d = d * 2;

                // We add two digits to handle
                // cases that make two digits
                // after doubling
                nSum += d / 10;
                nSum += d % 10;

                isSecond = !isSecond;
            }
            return (nSum % 10 == 0);
        }
    }
}
