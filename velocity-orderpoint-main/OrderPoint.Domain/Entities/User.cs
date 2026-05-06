using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbUsers")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string? User_email { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? User_password { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? User_rand_ID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? User_last_login { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? User_date_created { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? User_type { get; set; }
        public int? User_type_ID { get; set; }
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? First_name { get; set; }
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? Last_name { get; set; }
        [StringLength(50)]
        [Column(TypeName = "nchar(50)")]
        public string? User_logged_in_identifier { get; set; }
        public int? BCC_OrderConfirmation { get; set; }
    }
}
