﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface IVisitorReviewReportJob
    {
        Task SendTodayVisitorReviewReportAsync();

        Task SendTodayLoginDurationReportAsync();


        Task SendTodayLoginReportsAsync();


    }
}
