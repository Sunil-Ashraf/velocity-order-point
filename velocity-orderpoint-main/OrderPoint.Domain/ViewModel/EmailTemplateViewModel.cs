using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class EmailTemplateViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Required"), StringLength(200)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Body { get; set; }
        [Required(ErrorMessage = "Required")]
        public Guid? EmailTypeId { get; set; }
        public string? EmailType { get; set; }
        public bool IsDefault { get; set; }

        public DateTime? CreationTime { get; set; }
        public DateTime? UpdationTime { get; set; }

        public string? CreatedBy { get; set; }
        public string? Created { get; set; }

        public string? UpdatedBy { get; set; }
        public string? Updated { get; set; }
        public Int32? WholesalerID { get; set; }
    }
}
