using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbWholesaler")]
    public class Wholesaler
    {
        [Key]
        public int ID { get; set; }
        [Required, StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string Name { get; set; } = string.Empty;
        [StringLength(500)]
        [Column(TypeName = "varchar(500)")]
        public string? Logo { get; set; }
        public string? BannerImage { get; set; }
        [Required, StringLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string RandID { get; set; } = string.Empty;
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? Telephone { get; set; }
        [StringLength(500)]
        [Column(TypeName = "varchar(500)")]
        public string? Address { get; set; }
        [StringLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string? Email { get; set; }

        [StringLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string? LandingPageEmail { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? LandingPageTelephone { get; set; }

        
         
        public string? Description { get; set; }
     
       
        public string? WelcomeMessage { get; set; }


        public int Status { get; set; }
    }
}
