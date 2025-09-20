using BLL.ViewModels;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model_New.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CommonService
    {
        private UnitOfWork<MrAppDbNewContext> unitOfWork;

        private readonly ILogger<CommonService> _logger;




        private DbSet<TblSystemUser> SysUserDbSet;
        private DbSet<TblUser> UserDbSet;


        public CommonService()
        {
            unitOfWork = new UnitOfWork<MrAppDbNewContext>();




            var mrAppDbNewContext = new MrAppDbNewContext();
            SysUserDbSet = mrAppDbNewContext.TblSystemUsers;
            UserDbSet = mrAppDbNewContext.TblUsers;
        }

        //public List<TblUser> GetUsers(string UserName, string Password)
        //{
        //    var users = UserDbSet
        //        .Where(user => user.UserName == UserName && user.Password == Password)
        //        .ToList(); // Returns a list of users

        //    return users;
        //}

        public List<UserWithDetailsDto> GetUsers(string userName, string password)
        {
            var users = UserDbSet
                .Include(u => u.UserType)       // requires navigation property
                .Include(u => u.Location)       // requires navigation property
                .Where(u => u.UserName == userName && u.Password == password)
                .Select(u => new UserWithDetailsDto
                {
                    UserId = u.UserId,
                    EmpNo = u.EmpNo,
                    Password = u.Password,
                    EmpName = u.EmpName,
                    EmpEmail = u.EmpEmail,
                    HireDate = u.HireDate,
                    LineManager = u.LineManager,
                    IsActive = u.IsActive,
                    Gender = u.Gender,
                    LineManagerEmail = u.LineManagerEmail,
                    UserName = u.UserName,
                    CreatedDate = u.CreatedDate,
                    LocationName = u.Location.LocationName,
                    UserTypeName = u.UserType.UserTypeName
                })
                .ToList();

            return users;
        }



        public List<TblSystemUser> GetSystemUser(string Empno, bool isActive)
        {

            var SysUser = SysUserDbSet
           .Where(user => user.EmpNo == Empno && user.IsActive == isActive)
           .ToList();
            return SysUser;
        }


        //public List<TblDistributor> GetDistributorUser(string Empno, bool isActive)
        //{

        //    var SysUser = SysUserDbSet
        //   .Where(user => user.EmpNo == Empno && user.IsActive == isActive)
        //   .ToList();
        //    return SysUser;
        //}




    }
}
