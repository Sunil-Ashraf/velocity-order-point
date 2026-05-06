using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbAdmin")]
    public class Admin
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? First_name { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? Last_name { get; set; }
        public int? UserID { get; set; }
    }
}
