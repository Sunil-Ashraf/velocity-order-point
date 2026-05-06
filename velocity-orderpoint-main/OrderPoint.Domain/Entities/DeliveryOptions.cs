using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbDeliveryOptions")]
    public class DeliveryOptions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   // 👈 auto-increment
        public long Id { get; set; }

        public long WholesalerID { get; set; }

        public string WeekDay { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }
        public bool? IsUpdated { get; set; }
    }

}
