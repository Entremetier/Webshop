using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public static class MaxItemsInCart
    {
        // const ist default static
        private const int maxItemsInShoppingCart = 10;
        public static int MaxItemsInShoppingCart 
        { 
            get => maxItemsInShoppingCart; 
        }
    }
}
