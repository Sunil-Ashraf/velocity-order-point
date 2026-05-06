using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbCustomer")]
    public class Customer
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string WholesalerReference { get; set; } = string.Empty;

        public int WholesalerID { get; set; }

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string Name { get; set; } = string.Empty;
    }

}
