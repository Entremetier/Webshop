using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class MaxItemsInCart
    {
        private const int maxItemsInShoppingCart = 10;
        public int MaxItemsInShoppingCart 
        { 
            get => maxItemsInShoppingCart; 
        }
    }
}
