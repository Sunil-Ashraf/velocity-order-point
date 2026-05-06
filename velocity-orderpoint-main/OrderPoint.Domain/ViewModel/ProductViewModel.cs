using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class ProductsViewModel
    {
        public long Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? QuantityTypeId { get; set; }
        public string? QuantityTypeName { get; set; }
        public decimal Quantity { get; set; } = 1.000m;
        public string? WiegthedQuantity { get; set; }  
        
        public bool? IsItemInCart { get; set; } 
        public bool IsChecked { get; set; } 
        public decimal? Price { get; set; } 
    }
}
