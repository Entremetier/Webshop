using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public class PaymentService
    {
        public bool CreditCardValidation(decimal cardNumber)
        {
            int sum = 0;
            // cardnumber zu einem Array machen
            int[] cardNumberArray = cardNumber.ToString().Select(num => Convert.ToInt32(num)).ToArray();

            // Array durchlaufen
            //for (int i = 0; i < cardNumberArray.Length; i++)
            //{
            //    int digit = cardNumberArray[cardNumberArray.Length - i - 1];
            //    if (i % 2 == 1)
            //    {
            //        digit *= 2;
            //    }
            //    sum += digit > 9 ? digit - 9 : digit;
            //}

            //return sum % 10 == 0;

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
