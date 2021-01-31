using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Reminder.Service
{
    using Reminder = MyProfile.Entity.Model.Reminder;
    using ReminderDate = MyProfile.Entity.Model.ReminderDate;
    public class ReminderService
    {
        private IBaseRepository repository;
        private UserLogService userLogService;

        public ReminderService(IBaseRepository baseRepository)
        {
            this.repository = baseRepository;
            this.userLogService = new UserLogService(baseRepository);
        }

        public async Task<IList<ReminderEditModelView>> GetRimindersByDate(DateTime currentDate)
        {
            var currenUserID = UserInfo.Current.ID;

            var reminders = await repository
                .GetAll<ReminderDate>(x => x.Reminder.UserID == currenUserID
                    && x.Reminder.IsDeleted == false
                    && x.DateReminder == currentDate)
                 .Select(x => new ReminderEditModelView
                 {
                     ID = x.ReminderID,
                     DateReminder = currentDate, // x.DateReminder,
                     Description = x.Reminder.Description,
                     IsRepeat = x.Reminder.IsRepeat,
                     RepeatEvery = x.Reminder.RepeatEvery,
                     Title = x.Reminder.Title,
                     CssIcon = x.Reminder.CssIcon,
                     isShowForFilter = true,
                     isDeleted = false,
                 })
                 .ToListAsync();

            for (int i = 0; i < reminders.Count; i++)
            {
                reminders[i].DateReminderString = reminders[i].DateReminder.Value.ToString("dd.MM.yyyy");
                reminders[i].RepeatEveryName = LolalizatoinRepeat(reminders[i].RepeatEvery);
            }

            return reminders;
        }

        public int SetDoneForAllPastRemindersDate()
        {
            var now = DateTime.Now.ToUniversalTime();

            var reminderDates = repository.GetAll<ReminderDate>(x => x.Reminder.IsDeleted == false
                    && x.IsDone == false
                    && x.DateReminder.Date < now.Date)
                .ToList();

            for (int i = 0; i < reminderDates.Count; i++)
            {
                reminderDates[i].IsDone = true;

                if (i % 500 == 0)
                {
                    repository.Save();
                }
            }

            repository.Save();

            return reminderDates.Count;
        }

        public async Task<bool> CreateOrUpdate(ReminderEditModelView newReminder)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            try
            {
                if (newReminder.ID > 0)//Update
                {
                    var oldReminder = await repository.GetAll<Reminder>(x => x.ID == newReminder.ID
                            && x.UserID == currentUser.ID
                            && x.IsDeleted == false)
                        .FirstOrDefaultAsync();

                    if (newReminder.IsRepeat == false)
                    {
                        newReminder.RepeatEvery = null;
                    }

                    if (newReminder.IsRepeat == false && oldReminder.IsRepeat)
                    {
                        //remove until reminder.DateReminder
                        await repository.DeleteRangeAsync(await repository.GetAll<ReminderDate>(x => x.ReminderID == newReminder.ID
                            && x.Reminder.UserID == currentUser.ID
                            && x.Reminder.IsDeleted == false
                            && x.DateReminder >= newReminder.DateReminder).ToListAsync(),true);
                    }
                    else if (newReminder.IsRepeat && oldReminder.IsRepeat == false)
                    {
                        oldReminder.ReminderDates = GetReminderDates(newReminder, isCreateCurrentReminderDate: false);
                    }
                    else if (newReminder.IsRepeat
                        && oldReminder.IsRepeat
                        && newReminder.RepeatEvery != oldReminder.RepeatEvery)
                    {
                        await repository.DeleteRangeAsync(await repository.GetAll<ReminderDate>(x => x.ReminderID == newReminder.ID
                            && x.Reminder.UserID == currentUser.ID
                            && x.Reminder.IsDeleted == false
                            && x.DateReminder >= newReminder.DateReminder).ToListAsync(), true);
                        oldReminder.ReminderDates = GetReminderDates(newReminder, isCreateCurrentReminderDate: true);
                    }

                    oldReminder.Title = newReminder.Title;
                    oldReminder.Description = newReminder.Description;
                    oldReminder.DateReminder = newReminder.DateReminder;
                    oldReminder.DateEdit = now;
                    oldReminder.IsRepeat = newReminder.IsRepeat;
                    oldReminder.RepeatEvery = newReminder.RepeatEvery;
                    oldReminder.CssIcon = newReminder.CssIcon ?? "pe-7s-bell";

                    await repository.UpdateAsync(oldReminder, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Edit);
                }
                else
                {
                    var reminder = new Reminder();

                    reminder.UserID = currentUser.ID;
                    reminder.DateEdit = reminder.DateCreate = now;

                    reminder.Title = newReminder.Title;
                    reminder.Description = newReminder.Description;
                    reminder.DateReminder = newReminder.DateReminder;
                    reminder.IsRepeat = newReminder.IsRepeat;
                    reminder.RepeatEvery = newReminder.RepeatEvery;
                    reminder.CssIcon = newReminder.CssIcon ?? "pe-7s-bell";
                    reminder.ReminderDates = GetReminderDates(newReminder);

                    await repository.CreateAsync(reminder, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Create);

                    newReminder.ID = reminder.ID;

                }
            }
            catch (Exception ex)
            {
                return false;
            }

            newReminder.DateReminderString = newReminder.DateReminder.Value.ToString("dd.MM.yyyy");
            newReminder.RepeatEveryName = LolalizatoinRepeat(newReminder.RepeatEvery);

            return true;
        }

        public IQueryable<ReminderShortModelView> GetRemindersByDateRange(DateTime from, DateTime to)
        {
            var currenUserID = UserInfo.Current.ID;
            //from = from.AddSeconds(-1);
            //to = to.AddSeconds(1);

            return repository
                .GetAll<ReminderDate>(x => x.Reminder.UserID == currenUserID
                && x.Reminder.IsDeleted == false
                && x.DateReminder >= from
                && x.DateReminder <= to)
                .Select(x => new ReminderShortModelView
                {
                    CssIcon = x.Reminder.CssIcon,
                    DateReminder = x.DateReminder,
                    Description = x.Reminder.Description,
                    ReminderID = x.ReminderID,
                    Title = x.Reminder.Title,
                    IsRepeat = x.Reminder.IsRepeat,
                    IsDone = x.IsDone,
                });

            //return repository
            //    .GetAll<Reminder>(x => x.UserID == currenUserID
            //        && x.IsDeleted == false
            //        && x.DateReminder != null
            //        && x.ReminderDates != null
            //        && x.ReminderDates.Any(y => y.DateReminder >= from && y.DateReminder <= to))
            //     .Select(x => new ReminderShortModelView
            //     {
            //         ReminderID = x.ID,
            //         Description = x.Description,
            //         Title = x.Title,
            //         CssIcon = x.CssIcon,
            //         DateReminder = x.ReminderDates
            //            .FirstOrDefault(y => y.DateReminder >= from && y.DateReminder <= to)
            //            .DateReminder.Date
            //     });
        }

        public async Task<bool> RemoveOrRecovery(ReminderEditModelView reminderEdit, bool isDelete)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            var reminder = await repository.GetAll<Reminder>(x => x.ID == reminderEdit.ID
                    && x.UserID == currentUser.ID)
                .FirstOrDefaultAsync();

            reminder.DateEdit = now;
            reminder.IsDeleted = reminderEdit.isDeleted = isDelete;

            try
            {
                await repository.UpdateAsync(reminder, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Delete);

            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }



        public List<ReminderDate> GetReminderDates(ReminderEditModelView reminderEdit, bool isCreateCurrentReminderDate = true)
        {
            var now = DateTime.Now.ToUniversalTime();
            var dateReminder = reminderEdit.DateReminder.Value;
            List<ReminderDate> reminderDates = new List<ReminderDate>();

            if (isCreateCurrentReminderDate)
            {
                reminderDates.Add(new ReminderDate
                {
                    DateReminder = dateReminder,
                    IsDone = now.Date > dateReminder,
                    ReminderID = reminderEdit.ID
                });
            }

            if (reminderEdit.IsRepeat && reminderEdit.DateReminder != null)
            {
                if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Day))
                {
                    for (int i = 1; i < 765; i++)
                    {
                        dateReminder = reminderEdit.DateReminder.Value.AddDays(i);
                        reminderDates.Add(new ReminderDate
                        {
                            DateReminder = dateReminder,
                            IsDone = now.Date > dateReminder,
                            ReminderID = reminderEdit.ID,
                        });
                    }
                }
                else
                if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Week))
                {
                    for (int i = 1; i < 100; i++)
                    {
                        dateReminder = reminderEdit.DateReminder.Value.AddDays(i * 7);
                        reminderDates.Add(new ReminderDate
                        {
                            DateReminder = dateReminder,
                            IsDone = now.Date > dateReminder,
                            ReminderID = reminderEdit.ID
                        });
                    }
                }
                else if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Month))
                {
                    for (int i = 1; i < 24; i++)
                    {
                        reminderDates.Add(new ReminderDate
                        {
                            DateReminder = reminderEdit.DateReminder.Value.AddMonths(i),
                            IsDone = now.Date > dateReminder,
                            ReminderID = reminderEdit.ID
                        });
                    }
                }
                else if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Year))
                {
                    for (int i = 1; i < 10; i++)
                    {
                        dateReminder = reminderEdit.DateReminder.Value.AddYears(i);
                        reminderDates.Add(new ReminderDate
                        {
                            DateReminder = dateReminder,
                            IsDone = now.Date > dateReminder,
                            ReminderID = reminderEdit.ID
                        });
                    }
                }
            }

            return reminderDates;
        }

        private string LolalizatoinRepeat(string repeatEvery)
        {
            switch (repeatEvery)
            {
                case "Day":
                    return "День";
                case "Week":
                    return "Неделя";
                case "Month":
                    return "Месяц";
                case "Year":
                    return "Год";
                default:
                    return "";
            }
        }
    }
}
