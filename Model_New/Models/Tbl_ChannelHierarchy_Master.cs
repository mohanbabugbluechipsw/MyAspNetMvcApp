using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_ChannelHierarchy_Master
    {
        [Key]
        public int Id { get; set; }

        public string Master_Channel { get; set; }
        public string Master_Description { get; set; }

        public string Channel_Code { get; set; }
        public string Channel_Description { get; set; }

        public string Sub_Channel_Code { get; set; }
        public string Sub_Channel_Description { get; set; }

        public string Element_Code { get; set; }
        public string Element_Description { get; set; }

        public string Sub_Element_Code { get; set; }
        public string Sub_Element_Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
