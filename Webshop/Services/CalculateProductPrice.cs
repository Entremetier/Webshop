using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class CalculateProductPrice
    {
        public static decimal CalcPrice(Product product, List<Category> categoryAndTaxRate)
        {
            decimal price = 0;

            foreach (var cat in categoryAndTaxRate)
            {
                if (product.CategoryId == cat.Id)
                {
                    decimal netPriceDividedBy100 = product.NetUnitPrice / 100;
                    decimal taxes = netPriceDividedBy100 * cat.TaxRate;
                    price = product.NetUnitPrice + taxes;
                }
            }
            return price;
        }
    }
}
