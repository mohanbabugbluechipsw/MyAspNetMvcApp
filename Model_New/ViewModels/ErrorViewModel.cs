using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class ErrorViewModel
    {
        public Guid ErrorId { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Reference { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
        public string SupportContact { get; set; } = "Raneesh.Rajeevan@unilever.com";
    }
}
