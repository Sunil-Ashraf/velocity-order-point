using OrderPoint.Domain.ModifyIdentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    
    
    [Table("tblSystemSettings")]
    public class SystemSetting
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        [Required]
        [MaxLength(50)]
        public string ConfigurationName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

     
 

        public DateTime? CreationTime { get; set; }

        public long? CreatedBy { get; set; }

     

        public DateTime? UpdationTime { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
