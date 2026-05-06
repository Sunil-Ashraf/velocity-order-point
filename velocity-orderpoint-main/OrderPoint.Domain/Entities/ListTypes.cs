using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tblListTypes")]
    public class ListTypes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [MaxLength(200)]
        public string SystemName { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        [Column(TypeName = "char")]
        [StringLength(1)]
        public string Status { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsSystem { get; set; }
        public virtual ICollection<Lists>? Lists { get; set; }
    }
}
