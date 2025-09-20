using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class OutletViewSaveModel
    {
        public string Format { get; set; }
        public Dictionary<string, int> Md { get; set; }
        public Dictionary<string, int> Sachet { get; set; }
        public Dictionary<string, int> Category { get; set; }
    }

}
