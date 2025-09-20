using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_FSWSdetail
    {
        [Key] // <-- Add this
        public int Id { get; set; }
        public Guid FormId { get; set; }
        public string Section { get; set; }
        public string FieldName { get; set; }
        public string LabelText { get; set; }
        public string? ValueText { get; set; }
        public string? FileName { get; set; } 

        public byte[]? FileBytes { get; set; }
        public DateTime CreatedAt { get; set; }

        // New columns
        public string? rS_code { get; set; }

        public string? mr_code { get; set; }

        public string? Outlet_code { get; set; }

        public string? Outlet_type { get; set; }

        public string? srcode { get; set; }

        public string? route_name { get; set; }

    }

}
