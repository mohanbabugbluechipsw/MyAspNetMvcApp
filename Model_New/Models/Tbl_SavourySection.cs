using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_SavourySection
    {
        [Key]
        public int Id { get; set; }
        public string? Visible { get; set; }
        public string? DisplayOption { get; set; }
        public byte[]? Photo { get; set; }
        public string? PlanogramSeparate { get; set; }
        public string? BrandVariantsSeparate { get; set; }
        public string? NonUnileverPlacement { get; set; }


        public string? Rscode { get; set; }
        public string? MrCode { get; set; }
        public string? Outlet { get; set; }
        public string? OutletType { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;


    }
}
