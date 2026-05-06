using OrderPoint.Domain.ModifyIdentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OrderPoint.Domain.Entities
{
    [Table("tblLists")]
    public class Lists
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [MaxLength(500)]
        public string? Name { get; set; }
        [MaxLength(50)]
        public string? Code { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public long? ParentID { get; set; }
        public Int32? HierLevel { get; set; }
        public long? SortOrder { get; set; }
        public bool IsActive { get; set; }
        public bool? IsSystem { get; set; }
        [MaxLength(10)]
        public string? Color { get; set; }
        public long? CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public ApplicationUser User { get; set; }
        public Guid ListTypeId { get; set; }
        [ForeignKey(nameof(ListTypeId))]
        public virtual ListTypes? ListTypes { get; set; }
        public ICollection<EmailTemplate> EmailTemplateList { get; set; } = new HashSet<EmailTemplate>();




    }
}
