

using System;

namespace Model_New.Models
{
    public class ApplicationErrorLog
    {
        public Guid ErrorLogId { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string Endpoint { get; set; }
        public DateTime LoggedAt { get; set; }
        public string HttpMethod { get; set; }
        public string UserAgent { get; set; }
        public string UserIP { get; set; }
        public string Status { get; set; }

        // New fields
        public string RSCode { get; set; }
        public string OutletCode { get; set; }
        public string ReviewDetailsJson { get; set; }


        public string Username { get; set; }
        //public string UserRoles { get; set; }




    }
}

