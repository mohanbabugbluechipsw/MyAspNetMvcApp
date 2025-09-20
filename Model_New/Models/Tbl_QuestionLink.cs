using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class Tbl_QuestionLink
    {
        public int Id { get; set; }

        public int PreVisitQuestionId { get; set; }

        public int PostVisitQuestionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public bool IsActive { get; set; }
    }

}
