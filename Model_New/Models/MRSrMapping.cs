using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public  class Tbl_MRSrMapping
    {
  

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary key

        [Required]
        public int MrEmpNo { get; set; } // MR Employee Number

        [Required]
        public string MrName { get; set; }

        [Required]
        public string MR_Linked_SR_Details { get; set; }

        [Required]
        public string Rs_code { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Default to current time
    



}
    }
