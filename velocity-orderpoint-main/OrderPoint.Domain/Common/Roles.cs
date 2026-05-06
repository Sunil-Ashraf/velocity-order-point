using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Common
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Wholesaler = "Wholesaler";
        public const string Customer = "Customer";

        public static   List<string> RolesList = new()
    {
        Admin,
        Wholesaler,
        Customer
    };
    }
     
}
