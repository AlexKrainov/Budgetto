using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Counter;
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
                case BudgettoEntityType.Templates:
                    return repository.GetAll<Template>(x => x.UserID == currentUser.ID && x.IsDeleted == false).Count();
                default:
                    break;
            }

            return 0;
        }

        public async Task<int> GetCurrentEntityCountAsync(BudgettoEntityType entityType)
        {
            var currentUser = UserInfo.Current;
            switch (entityType)
            {
                case BudgettoEntityType.Limits:
                    return await repository.GetAll<Limit>(x => x.UserID == currentUser.ID && x.IsDeleted == false).CountAsync();
                case BudgettoEntityType.Reminders:
                    return await repository.GetAll<Reminder>(x => x.UserID == currentUser.ID && x.IsDeleted == false).CountAsync();
                case BudgettoEntityType.ToDoLists:
                    return await repository.GetAll<ToDoList>(x => x.ToDoListFolder.UserID == currentUser.ID && x.IsDeleted == false).CountAsync();
                case BudgettoEntityType.Templates:
                    return await repository.GetAll<Template>(x => x.UserID == currentUser.ID && x.IsDeleted == false).CountAsync();
                default:
                    break;
            }

            return 0;
        }

        public int HowMuchCanBeEntityCount(BudgettoEntityType entityType)
        {
            var currentUser = UserInfo.Current;

            return (repository.GetAll<UserEntityCounter>(x => x.UserID == currentUser.ID && x.EntityTypeID == (int)entityType).Select(x => x.AddedCount).FirstOrDefault())
                + repository.GetAll<PaymentCounter>(x => x.PaymentTariffID == currentUser.Payment.PaymentTariffID && x.EntityTypeID == (int)entityType).Select(x => x.CanBeCount).FirstOrDefault();
        }

        public async Task<int> HowMuchCanBeEntityCountAsync(BudgettoEntityType entityType)
        {
            var currentUser = UserInfo.Current;

            return (await repository.GetAll<UserEntityCounter>(x => x.UserID == currentUser.ID && x.EntityTypeID == (int)entityType).Select(x => x.AddedCount).FirstOrDefaultAsync())
                + await repository.GetAll<PaymentCounter>(x => x.PaymentTariffID == currentUser.Payment.PaymentTariffID && x.EntityTypeID == (int)entityType).Select(x => x.CanBeCount).FirstOrDefaultAsync();
        }

        public bool CanCreateEntity(BudgettoEntityType entityType)
        {
            return GetCurrentEntityCount(entityType) < HowMuchCanBeEntityCount(entityType);
        }
        public async Task<bool> CanCreateEntityAsync(BudgettoEntityType entityType)
        {
            return await GetCurrentEntityCountAsync(entityType) < await HowMuchCanBeEntityCountAsync(entityType);
        }

        public CounterViewModel GetCounterByEntity(BudgettoEntityType entityType)
        {
            var currentUser = UserInfo.Current;
            var userEntityCounter = repository.GetAll<UserEntityCounter>(x => x.UserID == currentUser.ID && x.EntityTypeID == (int)entityType)
                .Select(x => new UserEntityCounter
                {
                    AddedCount = x.AddedCount,
                    LastChanges = x.LastChanges
                })
                .FirstOrDefault();
            if (userEntityCounter == null)
            {
                userEntityCounter = new UserEntityCounter
                {
                    AddedCount = 0,
                    LastChanges = DateTime.Now //??
                };
            }

            return new CounterViewModel
            {
                AddedCount = userEntityCounter.AddedCount,
                CanBeCountByTariff = repository.GetAll<PaymentCounter>(x => x.PaymentTariffID == currentUser.Payment.PaymentTariffID && x.EntityTypeID == (int)entityType).Select(x => x.CanBeCount).FirstOrDefault(),
                CurrentCount = GetCurrentEntityCount(entityType),
                EntityType = entityType,
                LastChanges = userEntityCounter.LastChanges
            };
        }

        public async Task<CounterViewModel> GetCounterByEntityAsync(BudgettoEntityType entityType)
        {
            var currentUser = UserInfo.Current;
            var userEntityCounter = await repository.GetAll<UserEntityCounter>(x => x.UserID == currentUser.ID && x.EntityTypeID == (int)entityType)
                .Select(x => new UserEntityCounter
                {
                    AddedCount = x.AddedCount,
                    LastChanges = x.LastChanges
                })
                .FirstOrDefaultAsync();
            if (userEntityCounter == null)
            {
                userEntityCounter = new UserEntityCounter
                {
                    AddedCount = 0,
                    LastChanges = DateTime.Now //??
                };
            }

            return new CounterViewModel
            {
                AddedCount = userEntityCounter.AddedCount,
                CanBeCountByTariff = await repository.GetAll<PaymentCounter>(x => x.PaymentTariffID == currentUser.Payment.PaymentTariffID && x.EntityTypeID == (int)entityType).Select(x => x.CanBeCount).FirstOrDefaultAsync(),
                CurrentCount = await GetCurrentEntityCountAsync(entityType),
                EntityType = entityType,
                LastChanges = userEntityCounter.LastChanges
            };
        }
    }
}
