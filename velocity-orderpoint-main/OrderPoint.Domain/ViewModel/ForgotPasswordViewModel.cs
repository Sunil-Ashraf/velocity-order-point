using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
        public bool? IsResetByAdmin {get;set;}
    }
}
