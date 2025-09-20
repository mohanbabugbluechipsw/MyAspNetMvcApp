using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_Placement_descrption
    {


        [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(255)]
            public string FieldName { get; set; }

            [Required]
            [StringLength(500)]
            public string LabelText { get; set; }

            [StringLength(100)]
            public string? Section { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    }
}
