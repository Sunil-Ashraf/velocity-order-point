using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class SignupViewModel
    {
        public long? Id { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(200, ErrorMessage = "First name cannot be more than 200 characters.")]
        public string FirstName { get; set; }

        [MaxLength(200, ErrorMessage = "Last name cannot be more than 200 characters.")]
        [Required(ErrorMessage = "Required")]

        public string LastName { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
      ErrorMessage = "Invalid Email Address")]
        [MaxLength(200, ErrorMessage = "Email cannot be more than 200 characters.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Required")]
        [System.ComponentModel.DataAnnotations.Compare("EmailAddress")]
        [Display(Name = "Confirm Email ")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]

        public String ConfirmEmailAddress { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, MinimumLength = 14, ErrorMessage = "Password must be at least 14 characters long.")]
         
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9])[^\s]{14,}$",
    ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character.")]
        public string Password { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public string UserType { get; set; }
        public string? UserID { get; set; }
        public bool NotifyNewUser { get; set; } = true;
        public bool IsEdit { get; set; }
        public string? CreateFor { get; set; }
        public Int32? CustomerID { get; set; }
        public Int32? WholesalerID { get; set; }

        public bool? IsBCCOrderConfirmation { get; set; }
    }
}
