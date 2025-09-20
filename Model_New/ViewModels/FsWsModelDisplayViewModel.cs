using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class FsWsModelDisplayViewModel
    {
        public ModelDisplay ModelDisplay1 { get; set; }
        public ModelDisplay ModelDisplay2 { get; set; }
        public ModelDisplay ModelDisplay3 { get; set; }
    }



    public class ModelDisplay
    {
        public int Id { get; set; }
        public string VisibleToShopper { get; set; }
        public string DisplayOption { get; set; }
        public IFormFile DisplayPhoto { get; set; }
        public string UnileverSeparate { get; set; }
        public string BrandVariantsSeparate { get; set; }
        public string NonUnileverNotBetween { get; set; }
    }

}
