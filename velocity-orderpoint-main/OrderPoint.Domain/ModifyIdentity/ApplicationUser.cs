using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ModifyIdentity
{
    public class ApplicationUser : IdentityUser<long>
    {
       
        [StringLength(200)]
        public string? FirstName { get; set; }
        [StringLength(200)]
        public string? LastName { get; set; }
        public DateTime CreationTime { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        public bool NotifyNewUser { get; set; }
        public DateTime? LastLoginDate { get; set; } 
        public bool? BccOrderConfirmation { get; set; }
    }
}
