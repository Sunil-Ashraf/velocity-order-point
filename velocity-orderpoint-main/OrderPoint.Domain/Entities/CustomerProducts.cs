using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    
    [Table("tbCustomerProducts")]
    public class CustomerProducts
    {
        [Key, Column(Order = 1)]
        public long ID { get; set; }

         
        public int WholesalerID { get; set; }  
        public int CustomerID { get; set; }  
        public int ProductID { get; set; }
        public decimal? Price { get; set; }
    }
}
