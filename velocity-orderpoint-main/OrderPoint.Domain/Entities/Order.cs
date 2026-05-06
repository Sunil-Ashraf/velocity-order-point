using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbOrder")]
    public class Order
    {
        [Key]
        public int ID { get; set; }

        public int WholeSalerID { get; set; }

        public int CustomerID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OrderedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RequiredDate { get; set; }

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string Notes { get; set; } = string.Empty;

        public int Status { get; set; }

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string OrderNumber { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nchar(50)")]
        public string? Rand_ID { get; set; }
    }

}
