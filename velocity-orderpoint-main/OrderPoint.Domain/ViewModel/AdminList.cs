using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class AdminList
    {
        

        public long Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
