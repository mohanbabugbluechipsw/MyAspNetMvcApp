using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_ModelDisplay
    {


        [Key]
        public int Id { get; set; }
        public string? VisibleToShopper { get; set; }
        public string? DisplayOption { get; set; }
        public byte[]? DisplayPhoto { get; set; } // Store as byte[]
        public string? UnileverSeparate { get; set; }
        public string? BrandVariantsSeparate { get; set; }
        public string? NonUnileverNotBetween { get; set; }
        public string? ShelfStripAvailable { get; set; }


        public string? Rscode { get; set; }
        public string? MrCode { get; set; }
        public string? Outlet { get; set; }
        public string? OutletType { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    }
}
