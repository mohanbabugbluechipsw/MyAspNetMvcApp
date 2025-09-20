using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
 


    public class StoreVisitAnswerModel
    {
       



        public int QuestionId { get; set; }
        public string Text { get; set; } // Question text for display (optional for backend, but useful in frontend binding)

        public string ImageBase64 { get; set; } // Sent from frontend (Base64 string)

        // Optional: Use this if you later need form upload (not used in your current PreVisitJson)
        public IFormFile ImageFile { get; set; }

        public string BlobUrl { get; set; } // ✅ Store full URL with SAS

        public int IsNew { get; set; } // 1 = checked, 0 = unchecked


    }


}
