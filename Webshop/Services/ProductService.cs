using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Services
{
    public class ProductService
    {
        // Über _context arbeiten damit die Verbindung, beim filtern, zur DB nicht zu früh geschlossen wird
        private readonly LapWebshopContext _context;
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public ProductService(LapWebshopContext context, UserService userService, OrderService orderService)
        {
            _context = context;
            _orderService = orderService;
            _userService = userService;
        }

        // Hole Produkte aus der DB mit dem selben Hersteller, aber nicht das Produkt was wir gerade anschauen
        public async Task<List<Product>> GetProductsFromSameManufacturer(Product product)
        {
            using (var db = new LapWebshopContext())
            {
                List<Product> productList = await db.Products
                    .Where(x => x.ManufacturerId == product.ManufacturerId && x.Id != product.Id)
                    .ToListAsync();

                List<Product> choosenProductsWithSameManufacturer = ChooseProductsFromList(productList);
                return choosenProductsWithSameManufacturer;
            }
        }

        // Wähle zufällig zwei Produkte aus der gleichen Kategorie (Hersteller ist egal) aus, wenn es nur ein Produkt nimm nur das
        public async Task<List<Product>> GetProductsFromSameCategory(Product product)
        {
            using (var db = new LapWebshopContext())
            {
                List<Product> productList = await db.Products
                    .Where(x => x.CategoryId == product.CategoryId && x.Id != product.Id)
                    .ToListAsync();

                List<Product> choosenProductsWithSameCategory = ChooseProductsFromList(productList);
                return choosenProductsWithSameCategory;
            }
        }

        // Zufällig zwei Produkte auswählen
        private List<Product> ChooseProductsFromList(List<Product> productList)
        {
            List<Product> choosenProductsWithSameManufacturer = new List<Product>();
            Random random = new Random();

            if (productList.Count < 2)
            {
                Product product = productList.First();
                choosenProductsWithSameManufacturer.Add(product);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    List<int> usedNumbers = new List<int>();
                    int randomNumber = random.Next(1, productList.Count());

                    Product product = productList[randomNumber];
                    choosenProductsWithSameManufacturer.Add(product);
                    usedNumbers.Add(randomNumber);
                }
            }

            return choosenProductsWithSameManufacturer;
        }

        public async Task IncreaseCounterByOne(Product product)
        {
            using (var db = new LapWebshopContext())
            {
                // Produkt aus DB holen
                var productFromDb = await db.ProduktAufruves.Where(x => x.ProductId == product.Id)
                    .FirstOrDefaultAsync();

                // Wenn das Produkt nicht vorhanden ist
                if (productFromDb == null)
                {
                    // Produkt in die neue Tabelle schreiben
                    await AddProductToProduktAufrufe(product);
                    await IncreaseCounterByOne(product);
                }
                else
                {
                    // Produkt in der AnzahlAufrufe Tabelle suchen und um eins erhöhen
                    productFromDb.Calls = productFromDb.Calls + 1;

                    db.Update(productFromDb);
                    await db.SaveChangesAsync();
                }
            }
        }

        private async Task AddProductToProduktAufrufe(Product product)
        {
            using (var db = new LapWebshopContext())
            {
                ProduktAufrufe aufruf = new ProduktAufrufe()
                {
                    ProductId = product.Id,
                    Calls = 0,
                };

                await db.AddAsync(aufruf);
                await db.SaveChangesAsync();
            }
        }

        public async Task<int> GetAmountOfProductCalls(Product product)
        {
            using (var db = new LapWebshopContext())
            {
                var calls = await db.ProduktAufruves.Where(x => x.ProductId == product.Id)
                    .Select(x => x.Calls)
                    .FirstOrDefaultAsync();

                return calls;
            }
        }

        public async Task<Product> GetProductWithManufacturerAndCategory(int id)
        {
            using (var db = new LapWebshopContext())
            {
                var product = await db.Products.Include(m => m.Manufacturer)
                    .Include(c => c.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return product;
            }
        }

        public async Task<Product> GetProductWithManufacturer(int id)
        {
            using (var db = new LapWebshopContext())
            {
                Product product = await db.Products
                    .Include(m => m.Manufacturer)
                    .FirstOrDefaultAsync(p => p.Id == id);

                return product;
            }
        }

        public decimal CalcPrice(Product product, List<Category> categoryAndTaxRate)
        {
            decimal itemBruttoPrice = 0;

            foreach (var cat in categoryAndTaxRate)
            {
                if (product.CategoryId == cat.Id)
                {
                    itemBruttoPrice = product.NetUnitPrice / 100 * (100 + cat.TaxRate);
                }
            }
            // Auf 2 Nachkommastellen runden
            return Math.Round(itemBruttoPrice, 2);
        }

        public async Task<List<SelectListItem>> GetMaxItemAmount(Product product, string email)
        {
            List<SelectListItem> itemAmount = new List<SelectListItem>();

            using (var db = new LapWebshopContext())
            {
                // Wenn user angemeldet ist
                if (email != null)
                {
                    //User aus DB holen
                    Customer customer = await _userService.GetCurrentUser(email);

                    // Die offene Bestellung des Users aus DB holen
                    var order = await _orderService.GetOrder(customer);

                    // Menge vom Product die schon im Warenkorb ist
                    int productAmountInCart = await db.OrderLines.Where(x => x.ProductId == product.Id && order.Id == x.OrderId && order.DateOrdered == null)
                        .Select(x => x.Amount)
                        .FirstOrDefaultAsync();

                    // Wenn schon 10 Stk von einem Product im Warenkorb sind
                    if (productAmountInCart >= MaxItemsInCart.MaxItemsInShoppingCart)
                    {
                        //itemAmount.Add(new SelectListItem { Value = "0", Text = "0" });
                    }
                    else
                    {
                        int amountCustomerCanAdd = MaxItemsInCart.MaxItemsInShoppingCart - productAmountInCart;
                        for (int i = 1; i <= amountCustomerCanAdd; i++)
                        {
                            itemAmount.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                        }
                    }
                }
                // Wenn user nicht angemeldet ist
                else
                {
                    for (int i = 1; i <= MaxItemsInCart.MaxItemsInShoppingCart; i++)
                    {
                        itemAmount.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                    }
                }
                return itemAmount;
            }
        }

        public IQueryable<Product> FilterList(string searchString, string categorie, string manufacturer)
        {
            IQueryable<Product> products = null;

            // categorie und manufacturer können "0" sein wenn im DDL "Alle Kategorien/Hersteller" ausgewählt wird
            if (categorie == "0")
            {
                categorie = null;
            }

            if (manufacturer == "0")
            {
                manufacturer = null;
            }

            if (searchString == null && categorie != null && manufacturer == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == categorie);
            }
            else if (searchString != null && categorie != null && manufacturer == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == categorie && p.ProductName.Contains(searchString) ||
                    p.Category.Name == categorie && p.Manufacturer.Name.Contains(searchString) ||
                    p.Category.Name == categorie && p.Description.Contains(searchString));
            }
            else if (searchString == null && categorie == null && manufacturer != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Manufacturer.Name == manufacturer);
            }
            else if (searchString != null && categorie == null && manufacturer != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.ProductName.Contains(searchString) && p.Manufacturer.Name == manufacturer ||
                    p.Manufacturer.Name.Contains(searchString) && p.Manufacturer.Name == manufacturer ||
                    p.Description.Contains(searchString) && p.Manufacturer.Name == manufacturer);
            }
            else if (searchString == null && categorie != null && manufacturer != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == categorie && p.Manufacturer.Name == manufacturer);
            }
            else if (searchString != null && categorie != null && manufacturer != null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == categorie && p.Manufacturer.Name == manufacturer && p.ProductName.Contains(searchString) ||
                    p.Category.Name == categorie && p.Manufacturer.Name == manufacturer && p.Manufacturer.Name.Contains(searchString) ||
                    p.Category.Name == categorie && p.Manufacturer.Name == manufacturer && p.Description.Contains(searchString));
            }
            else if (searchString != null && categorie == null && manufacturer == null)
            {
                products = _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .Where(p => p.Description.Contains(searchString) || p.ProductName.Contains(searchString) || p.Manufacturer.Name.Contains(searchString));
            }
            else
            {
                // Wenn die Liste beim Start befüllt wird oder bei der Suche keine Parameter angegeben werden
                products = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer);
            }

            return products;
        }
    }
}
