using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Entities
{
    [Table("tbSessions")]
    public class Session
    {
        [Key]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Id { get; set; } = null!;
        public int? access { get; set; }
        [Column(TypeName = "text")]
        public string? data { get; set; }
    }
}
