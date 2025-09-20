﻿using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IStoreVisitRepository
    {
        Task<IEnumerable<StoreVisitLogDto>> GetTodayVisitLogsAsync();
        Task RunInsertProcedureAsync();
    }
}
