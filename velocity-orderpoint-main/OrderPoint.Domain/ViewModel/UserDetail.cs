using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    
    public class UserDetail
    {
        public long ID { get; set; }                  // or use Guid/int based on your Identity setup
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
      
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastLogin { get; set; }
        public long? CustomerID { get; set; }
        public long? WholesalerID { get; set; }
        public bool? IsBCCToOrderEmail { get; set; }
       
    }
    
}
