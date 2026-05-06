using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OrderPoint.Domain.Entities
{
    [Table("tbProduct")]
    public class Product
    {
        [Key, Column(Order = 0, TypeName = "varchar(100)")]
        [StringLength(100)]
        public string WholesalerID { get; set; } = string.Empty;

        [Key, Column(Order = 1)]
        public long ID { get; set; }

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string Description { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public int QuantityType { get; set; }
        [Precision(18, 2)]
        public decimal? Price { get; set; }
    }
}
