using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    [MetadataTypeAttribute(typeof(Category.Metadata))]
    public partial class Category
    {
        internal sealed class Metadata
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public decimal TaxRate { get; set; }
        }
    }
}
