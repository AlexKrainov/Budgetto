using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Progress;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Progress.Service
{
    public class ProgressService
    {
        private IBaseRepository repository;
        private UserLogService userLogService;

        public ProgressService(IBaseRepository repository,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
        }

        public List<ProgressItemViewModel> SetAndGetFinancialLiteracyMonthProgress(decimal totalEarnings, decimal totalSpendings, decimal totalInvest, int countRecordsDays)
        {
            var currentUser = UserInfo.Current;
            List<ProgressItemViewModel> progressItems = new List<ProgressItemViewModel>();

            #region ProgressItemTypeEnum.Investing10Percent

            decimal investPercent = 0;
            decimal needToBePercent = decimal.Parse(repository.GetAll<MyProfile.Entity.Model.Progress>(x =>
                x.UserID == currentUser.ID
                && x.IsActive
                && x.ParentProgressID != null
                && x.ProgressTypeID == (int)ProgressTypeEnum.FinancialLiteracyMonth
                && x.ProgressItemTypeID == (int)ProgressItemTypeEnum.Investing10Percent)
                .FirstOrDefault().NeedToBeValue);
            if (totalInvest > 0 && totalEarnings > 0)
            {
                investPercent = Math.Round(((totalInvest - totalEarnings) / totalEarnings * 100), 1);

                if (investPercent < 0)
                {
                    investPercent += 100;
                }
            }
            SetCompleteProgressItemType(currentUser.ID, ProgressTypeEnum.FinancialLiteracyMonth, ProgressItemTypeEnum.Investing10Percent, isComplete: investPercent >= needToBePercent, investPercent.ToString());
            #endregion

            #region  ProgressItemTypeEnum.EarnMoreThanSpend

            SetCompleteProgressItemType(currentUser.ID, ProgressTypeEnum.FinancialLiteracyMonth, ProgressItemTypeEnum.EarnMoreThanSpend, isComplete: totalEarnings >= totalSpendings);

            #endregion

            #region ProgressItemTypeEnum.CreateRecords70PercentAMonth

            int needToBeCount = int.Parse(repository.GetAll<MyProfile.Entity.Model.Progress>(x =>
                x.UserID == currentUser.ID
                && x.IsActive
                && x.ParentProgressID != null
                && x.ProgressTypeID == (int)ProgressTypeEnum.FinancialLiteracyMonth
                && x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateRecords70PercentAMonth)
                .FirstOrDefault().NeedToBeValue);
            SetCompleteProgressItemType(currentUser.ID, ProgressTypeEnum.FinancialLiteracyMonth, ProgressItemTypeEnum.CreateRecords70PercentAMonth, isComplete: countRecordsDays >= needToBeCount, countRecordsDays.ToString());
            #endregion

            #region  Get
            var progresses = repository.GetAll<Entity.Model.Progress>(x =>
                x.UserID == currentUser.ID
                && x.IsActive
                && x.ParentProgressID != null
                && x.ProgressTypeID == (int)ProgressTypeEnum.FinancialLiteracyMonth)
                .Select(x => new
                {
                    x.ID,
                    x.IsComplete,
                    x.DateComplete,
                    x.ProgressTypeID,
                    x.ProgressItemTypeID,
                    x.NeedToBeValue,
                    x.UserDateEdit,
                })
                .ToList();

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "pe-7s-piggy",
                Tooltip = "Ивестировать 10% от дохода",
                Description = "Сейчас проинвестированно: " + investPercent + "%",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.Investing10Percent).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.Investing10Percent).DateComplete,
            });
            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "pe-7s-graph1",
                Tooltip = "Доходы больше чем расходы",
                Description = "",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.EarnMoreThanSpend).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.EarnMoreThanSpend).DateComplete,
            });
            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "pe-7s-date",
                Tooltip = "Систематически заносить раходы/доходы",
                Description = countRecordsDays + " дней из " + needToBeCount + " дня",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateRecords70PercentAMonth).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateRecords70PercentAMonth).DateComplete,
            });
            #endregion

            return progressItems;
        }

        public List<ProgressItemViewModel> GetIntroductoryProgress()
        {
            var currentUser = UserInfo.Current;
            List<ProgressItemViewModel> progressItems = new List<ProgressItemViewModel>();

            var progresses = repository.GetAll<MyProfile.Entity.Model.Progress>(x =>
                x.UserID == currentUser.ID
                && x.IsActive
                && x.ParentProgressID != null
                && x.ProgressTypeID == (int)ProgressTypeEnum.Introductory)
                .Select(x => new
                {
                    x.ID,
                    x.IsComplete,
                    x.DateComplete,
                    x.ProgressTypeID,
                    x.ProgressItemTypeID
                })
                .ToList();

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "ion ion-md-add",
                Tooltip = "Добавить расходы/доходы",
                Description = "Записи основной компонент ведения личных финансов. Также вы можете добавить хештего через знаки # или !, известные бренды будут цеплятся автоматически",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateRecord).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateRecord).DateComplete,
                OnClick = "$('#addRecord').click()"
            });

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "lnr lnr-frame-expand",
                Tooltip = "Добавить лимит",
                Description = "Лимиты нужно, чтобы планировать бюджет и не выходить за рамки запланированного",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateLimit).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateLimit).DateComplete,
                Href = "/Limit/List"
            });

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "ion ion-ios-notifications-outline",
                Tooltip = "Добавить уведомление для лимита или напоминания",
                Description = "Уведомление, нужно например, чтобы вам на почту/телеграм/на сайте пришло уведомление о",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateNotification).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateNotification).DateComplete,
                Href = "/Limit/List"
            });

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "pe-7s-alarm",
                Tooltip = "Добавить напоминание",
                Description = "Напоминания позволят не забыть о самых важных запланированных делах, днях рождения, оплаты счетов и тд. Так же добавляйте уведомления на телефон, чтобы точно не пропустить",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateReminder).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateReminder).DateComplete,
                OnClick = "$('#reminder-history').click(); $('#add-reminder').click();"
            });

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "pe-7s-albums",
                Tooltip = "Добавить категорию",
                Description = "Категория",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateSection).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateSection).DateComplete,
                Href = "/Section/Edit"
            });

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "pe-7s-folder",
                Tooltip = "Добавить группу категорий",
                Description = "Группа категорий, нужно для объединения категорий по смыслу или действию. Например, в группу катигорий Еда/Продукты, может входить продукты, фастфуд, рестораны и тд.",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateArea).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateArea).DateComplete,
                Href = "/Section/Edit"
            });

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "lnr lnr-layers",
                Tooltip = "Добавить или отредактируйте шаблон",
                Description = "Шаблоны нужны, чтобы объединять по вашем предпочтениям категории",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateOrEditTemplate).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateOrEditTemplate).DateComplete,
                Href = "/Template/List"
            });

            progressItems.Add(new ProgressItemViewModel
            {
                CssClass = "pe-7s-cash",
                Tooltip = "Добавить счет, вклад, наличные и тд",
                Description = "Вы можете видеть все свои счета сразу...",
                IsComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateAccount).IsComplete,
                DateComplete = progresses.FirstOrDefault(x => x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateAccount).DateComplete,
                OnClick = "MainAccountVue.edit(null, false)"
            });

            return progressItems;
        }

        public async Task<int> SetCompleteProgressItemTypeAsync(Guid userID, ProgressTypeEnum progressTypeEnum, ProgressItemTypeEnum progressItemTypeEnum)
        {
            var progress = await repository.GetAll<Entity.Model.Progress>(x =>
                x.UserID == userID
                && x.ParentProgressID != null
                && x.ProgressTypeID == (int)progressTypeEnum
                && x.ProgressItemTypeID == (int)progressItemTypeEnum)
                .FirstOrDefaultAsync();

            if (!progress.IsComplete)
            {
                progress.IsComplete = true;
                progress.DateComplete = DateTime.Now.ToUniversalTime();
                await repository.UpdateAsync(progress, true);

                if (await repository.AnyAsync<Entity.Model.Progress>(x =>
                            x.UserID == userID
                            && x.ParentProgressID == null
                            && x.ProgressTypeID == (int)progressTypeEnum
                            && x.IsComplete == false)
                    &&
                    await repository.GetAll<Entity.Model.Progress>(x =>
                            x.UserID == userID
                            && x.ParentProgressID != null
                            && x.ProgressTypeID == (int)progressTypeEnum
                            && x.IsComplete == false).CountAsync() == 0)
                {
                    var mainProgress = await repository.GetAll<Entity.Model.Progress>(x =>
                            x.UserID == userID
                            && x.ParentProgressID == null
                            && x.ProgressTypeID == (int)progressTypeEnum)
                        .FirstOrDefaultAsync();

                    mainProgress.IsComplete = true;
                    mainProgress.DateComplete = DateTime.Now.ToUniversalTime();
                    await repository.UpdateAsync(mainProgress, true);

                    var currentUser = UserInfo.Current;
                    //currentUser.IsCompleteIntroductoryProgress = true;
                    //при следующем входе, должен скрыться начальный прогресс бар
                    // await UserInfo.AddOrUpdate_Authenticate(currentUser);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Progress_Introductory_Finish);
                }
            }
            return 1;
        }
        public int SetCompleteProgressItemType(Guid userID, ProgressTypeEnum progressTypeEnum, ProgressItemTypeEnum progressItemTypeEnum, bool isComplete = true, string currentValue = null)
        {
            var progress = repository.GetAll<Entity.Model.Progress>(x =>
                x.UserID == userID
                && x.ParentProgressID != null
                && x.ProgressTypeID == (int)progressTypeEnum
                && x.ProgressItemTypeID == (int)progressItemTypeEnum)
                .FirstOrDefault();

            progress.IsComplete = isComplete;
            progress.CurrentValue = currentValue;
            progress.DateComplete = isComplete ? DateTime.Now.ToUniversalTime() : (DateTime?)null;

            repository.Update(progress, true);

            if (repository.Any<Entity.Model.Progress>(x =>
                        x.UserID == userID
                        && x.ParentProgressID == null
                        && x.ProgressTypeID == (int)progressTypeEnum
                        && x.IsComplete == false)
                &&
                repository.GetAll<Entity.Model.Progress>(x =>
                        x.UserID == userID
                        && x.ParentProgressID != null
                        && x.ProgressTypeID == (int)progressTypeEnum
                        && x.IsComplete == false).Count() == 0)
            {
                var mainProgress = repository.GetAll<Entity.Model.Progress>(x =>
                        x.UserID == userID
                        && x.ParentProgressID == null
                        && x.ProgressTypeID == (int)progressTypeEnum)
                    .FirstOrDefault();

                mainProgress.IsComplete = isComplete;
                mainProgress.DateComplete = DateTime.Now.ToUniversalTime();
                repository.Update(mainProgress, true);

                var currentUser = UserInfo.Current;
                //currentUser.IsCompleteIntroductoryProgress = true;
                //при следующем входе, должен скрыться начальный прогресс бар
                // await UserInfo.AddOrUpdate_Authenticate(currentUser);
                userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Progress_Introductory_Finish);
            }
            //}
            return 1;
        }
    }
}
