using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbOrderLineItem")]
    public class OrderLineItem
    {
        [Key]
        public int ID { get; set; }

        public int OrderID { get; set; }

        public int ProductID { get; set; }
        [Column(TypeName = "float")]
        public double Quantity { get; set; }

        public int QuantityType { get; set; }

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string Notes { get; set; } = string.Empty;
    }

}
