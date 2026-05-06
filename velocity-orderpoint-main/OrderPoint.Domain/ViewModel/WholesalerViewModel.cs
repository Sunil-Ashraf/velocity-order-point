using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class WholesalerViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string RandID { get; set; }
        public string ImagePath { get; set; }
        public Int32 NoOfUser { get; set; }
        public String Status { get; set; }
    }
    public class AddEditWholesalerModel
    {
        public long? ID { get; set; }

        [Required(ErrorMessage = "Required"), StringLength(200)]
        public string? Name { get; set; }

        [StringLength(450, ErrorMessage = "Maximum 450 characters allowed.")]
        public string? Address { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid Email Address")]
        [MaxLength(200, ErrorMessage = "Email cannot be more than 200 characters.")]
        public string? Email { get; set; }

        [StringLength(15, ErrorMessage = "Maximum 15 characters allowed.")]
        [RegularExpression(@"^\+?[0-9\s\-()]{7,20}$", ErrorMessage = "Invalid telephone number format.")]
        public string? Telephone { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid Email Address")]
        [MaxLength(200, ErrorMessage = "Email cannot be more than 200 characters.")]
        public string? LandingPageEmail { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(15, ErrorMessage = "Maximum 15 characters allowed.")]
        [RegularExpression(@"^\+?[0-9\s\-()]{7,20}$", ErrorMessage = "Invalid telephone number format.")]
        public string? LandingPageTelephone { get; set; }

        public string? ImagePath { get; set; }
        public string? BannerImagePath { get; set; }

        [Required(ErrorMessage = "Required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(1000, ErrorMessage = "Maximum 1000 characters allowed.")]
        public string? WelcomeMessage { get; set; }

        public IFormFile? ProfilePicture { get; set; }
        public IFormFile? BannerPicture { get; set; }
    }
}