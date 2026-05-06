using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbCategory")]
    public class Category
    {
        [Key, Column(Order = 0)]
        public int WholesalerID { get; set; }
        [Key, Column(Order = 1)]
        public int ID { get; set; }
        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string Name { get; set; } = null!;
    }
}
