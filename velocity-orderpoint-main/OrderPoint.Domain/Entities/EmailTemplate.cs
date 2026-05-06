using OrderPoint.Domain.ModifyIdentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tblEmailTemplates")]
    public class EmailTemplate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Required"), StringLength(200)]
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Guid? EmailTypeId { get; set; }
        public bool IsDefault { get; set; }

        public DateTime? CreationTime { get; set; }
        public DateTime? UpdationTime { get; set; }


        public long? CreatedBy { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public ApplicationUser? Created { get; set; }

        public long? UpdatedBy { get; set; }
        [ForeignKey(nameof(UpdatedBy))]
        public ApplicationUser? Updated { get; set; }

        [ForeignKey(nameof(EmailTypeId))]
        [InverseProperty("EmailTemplateList")]
        public Lists? EmailTypeList { get; set; }

        public Int32? WholesalerID { get; set; }
    }

}
