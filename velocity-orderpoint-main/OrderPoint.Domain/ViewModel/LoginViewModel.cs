using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage ="Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        public string? RoleName { get; set; }
        public bool? IsAdminPortal { get; set; }
        public bool? IsCustomerPortal { get; set; }
        public Int32? WholesalerID { get; set; }

    }
    public class ForgotModel
    {
        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        public string Email { get; set; }
      

      
    }
}
