using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class AdminViewModel
    {

        [Required(ErrorMessage = "Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(100, MinimumLength = 14, ErrorMessage = "Password must be at least 14 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{14,}$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one number.")]
        public string Password { get; set; }

        public bool IsNotifyNewuser { get; set; }


        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Email")]
        [Display(Name = "Confirm Email ")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String ConfirmEmailAddress { get; set; }



        //[DataType(DataType.Password)]
        //[Display(Name = "New Password ")]
        //public String ResetPassword { get; set; }

        //[Required(ErrorMessageResourceType = typeof(WebUI.NassauTennis.MultipleLanguageResources.Js.SignUpValidation), ErrorMessageResourceName = "Required", ErrorMessage = null)]
        //[DataType(DataType.Password)]
        //[System.ComponentModel.DataAnnotations.Compare("ResetPassword")]
        //[Display(Name = "Confirm Password ")]
        //public String ConfirmResetPassword { get; set; }
    }

    public class EditUseViewModel
    {
        public long ID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        
        public string? UserType { get; set; }
        public bool isBccEmail { get; set; }

    }
}