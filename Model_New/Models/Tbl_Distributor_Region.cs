using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_Distributor_Region
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

     
        [MaxLength(50)]
        public string Region { get; set; }

     
        [MaxLength(50)]
        public string Area { get; set; }

        [Required]
        public int RS_Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string RS_Name { get; set; }
    }
}


