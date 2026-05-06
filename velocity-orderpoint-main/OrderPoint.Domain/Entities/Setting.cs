using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbSettings")]
    public class Setting
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string SMTPHostName { get; set; } = string.Empty;

        public int SMTPPort { get; set; }

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string SMTPUserName { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string SMTPPassword { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string ForgottenPasswordFromEmailAddress { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string FromEmailAddressName { get; set; } = "Order-Point";

        [Required, StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string SecurityLayer { get; set; } = "0";
    }
}
