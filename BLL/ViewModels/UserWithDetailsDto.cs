using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ViewModels
{
    public class UserWithDetailsDto
    {
        public int UserId { get; set; }
        public string EmpNo { get; set; }
        public string Password { get; set; }
        public string EmpName { get; set; }
        public string EmpEmail { get; set; }
        public DateTime? HireDate { get; set; }
        public string LineManager { get; set; }
        public bool? IsActive { get; set; }
        public string Gender { get; set; }
        public string LineManagerEmail { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }

        // Extra from joins
        public string LocationName { get; set; }
        public string UserTypeName { get; set; }
    }

}
