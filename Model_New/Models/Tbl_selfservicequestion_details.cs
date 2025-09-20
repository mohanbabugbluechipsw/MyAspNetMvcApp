using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_selfservicequestion_details
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Section { get; set; }

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; }

        [Required]
        [StringLength(255)]
        public string LabelText { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
