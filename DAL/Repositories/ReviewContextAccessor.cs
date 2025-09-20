using DAL.IRepositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ReviewContextAccessor : IReviewContextAccessor
    {
        public ReviewContextDto Value { get; set; }
    }
}
