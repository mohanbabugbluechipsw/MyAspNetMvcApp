using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_Placement_EmergencyLM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Section { get; set; }

        [MaxLength(100)]
        public string FieldName { get; set; }

        [MaxLength(255)]
        public string LabelText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
