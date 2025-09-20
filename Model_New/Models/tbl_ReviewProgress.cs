using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    [Table("tbl_ReviewProgress")]
    public class tbl_ReviewProgress
    {
        [Key]
        public int ProgressId { get; set; }
        public Guid ReviewId { get; set; }
        [MaxLength(50)]
        public string OutletCode { get; set; }
        [MaxLength(100)]
        public string UserId { get; set; }
        [MaxLength(50)]
        public string Stage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
