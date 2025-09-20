using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_SachetHanger
    {

        [Key]
        public int Id { get; set; }
        public string? SachetHangerAvailable { get; set; }

        //public LaundrySection? Laundry { get; set; }
        //public SavourySection? Savoury { get; set; }
        //public HfdSection? Hfd { get; set; }


   
        public string? Rscode { get; set; }
        public string? MrCode { get; set; }
        public string? Outlet { get; set; }
        public string? OutletType { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        // Foreign Key References
        public int? LaundryId { get; set; }
        public int? SavouryId { get; set; }
        public int? HfdId { get; set; }

        // Navigation Properties
        [ForeignKey("LaundryId")]
        public virtual Tbl_LaundrySection? Laundry { get; set; }

        [ForeignKey("SavouryId")]
        public virtual Tbl_SavourySection? Savoury { get; set; }

        [ForeignKey("HfdId")]
        public virtual Tbl_HfdSection? Hfd { get; set; }

        // Newly added properties
       

        // Automatically set creation date
   
    }
}
