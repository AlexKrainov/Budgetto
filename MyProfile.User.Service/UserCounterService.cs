using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Payment;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.User.Service
{
    public class UserCounterService
    {
        private IBaseRepository repository;
        private UserLogService userLogService;

        public UserCounterService(IBaseRepository repository,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
        }

        //public async Task<bool> AddEntity(UserInfoModel currentUser, BudgettoEntityType entityType)
        //{
        //    //var counter = currentUser.Counters.FirstOrDefault(x => x.EntityType == entityType);
        //    //counter
        //    //await UserInfo.AddOrUpdate_Authenticate(currentUser);
        //}

        public int GetCurrentEntityCount(BudgettoEntityType entityType)
        {
            var currentUser = UserInfo.Current;
            switch (entityType)
            {
                case BudgettoEntityType.Limits:
                    return repository.GetAll<Limit>(x => x.UserID == currentUser.ID && x.IsDeleted == false).Count();
                case BudgettoEntityType.Reminders:
                    return repository.GetAll<Reminder>(x => x.UserID == currentUser.ID && x.IsDeleted == false).Count();
                case BudgettoEntityType.ToDoLists:
                    return repository.GetAll<ToDoList>(x => x.ToDoListFolder.UserID == currentUser.ID && x.IsDeleted == false).Count();
                default:
                    break;
            }

            return 0;
        }
    }
}
