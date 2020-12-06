using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
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
                .GetAll<Reminder>(x => x.UserID == currenUserID
                    && x.IsDeleted == false
                    && x.DateReminder != null
                    && (x.DateReminder.Value.Date == currentDate.Date
                        || x.ReminderDates.Any(y => y.DateReminder.Date == currentDate.Date)))
                 .Select(x => new ReminderEditModelView
                 {
                     ID = x.ID,
                     DateCreate = x.DateCreate,
                     DateEdit = x.DateEdit,
                     DateReminder = x.DateReminder,
                     Description = x.Description,
                     IsRepeat = x.IsRepeat,
                     RepeatEvery = x.RepeatEvery,
                     Title = x.Title,
                     CssIcon = x.CssIcon,
                     isShowForFilter = true,
                     isDeleted = false,
                 })
                 .ToListAsync();
            return reminders;
        }


        public async Task<bool> CreateOrUpdate(ReminderEditModelView reminderEdit)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            var reminderDates = GetReminderDates(reminderEdit);

            try
            {
                if (reminderEdit.ID > 0)//Update
                {
                    var reminder = await repository.GetAll<Reminder>(x => x.ID == reminderEdit.ID
                    && x.UserID == currentUser.ID
                    && x.IsDeleted == false)
                        .FirstOrDefaultAsync();

                    reminder.Title = reminderEdit.Title;
                    reminder.Description = reminderEdit.Description;
                    reminder.DateReminder = reminderEdit.DateReminder;
                    reminderEdit.DateEdit = reminder.DateEdit = now;
                    reminder.IsRepeat = reminderEdit.IsRepeat;
                    reminder.RepeatEvery = reminderEdit.RepeatEvery;
                    reminder.CssIcon = reminderEdit.CssIcon ?? "pe-7s-bell";

                    if (reminder.ReminderDates.Count() != 0)
                    {
                        await repository.DeleteRangeAsync(reminder.ReminderDates, true);
                    }

                    reminder.ReminderDates = reminderDates;

                    await repository.UpdateAsync(reminder, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Edit);
                }
                else
                {
                    var reminder = new Reminder();

                    reminder.UserID = currentUser.ID;
                    reminderEdit.DateEdit = reminderEdit.DateCreate = reminder.DateEdit = reminder.DateCreate = now;

                    reminder.Title = reminderEdit.Title;
                    reminder.Description = reminderEdit.Description;
                    reminder.DateReminder = reminderEdit.DateReminder;
                    reminder.IsRepeat = reminderEdit.IsRepeat;
                    reminder.RepeatEvery = reminderEdit.RepeatEvery;
                    reminder.CssIcon = reminderEdit.CssIcon ?? "pe-7s-bell";
                    reminder.ReminderDates = reminderDates;

                    await repository.CreateAsync(reminder, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Create);

                    reminderEdit.ID = reminder.ID;

                }
            }
            catch (Exception ex)
            {
                return false;
            }

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
                    IsRepeat = x.Reminder.IsRepeat
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
            reminder.IsDeleted = isDelete;

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



        public List<ReminderDate> GetReminderDates(ReminderEditModelView reminderEdit)
        {
            List<ReminderDate> reminderDates = new List<ReminderDate>();

            if (reminderEdit.DateReminder != null)
            {
                reminderDates.Add(new ReminderDate
                {
                    DateReminder = reminderEdit.DateReminder.Value,
                    IsDone = false,
                    ReminderID = reminderEdit.ID
                });
            }

            if (reminderEdit.IsRepeat && reminderEdit.DateReminder != null)
            {
                if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Day))
                {
                    for (int i = 1; i < 765; i++)
                    {
                        reminderDates.Add(new ReminderDate
                        {
                            DateReminder = reminderEdit.DateReminder.Value.AddDays(i),
                            IsDone = false,
                            ReminderID = reminderEdit.ID
                        });
                    }
                }
                else
                if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Week))
                {
                    for (int i = 1; i < 100; i++)
                    {
                        reminderDates.Add(new ReminderDate
                        {
                            DateReminder = reminderEdit.DateReminder.Value.AddDays(i * 7),
                            IsDone = false,
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
                            IsDone = false,
                            ReminderID = reminderEdit.ID
                        });
                    }
                }
                else if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Year))
                {
                    for (int i = 1; i < 10; i++)
                    {
                        reminderDates.Add(new ReminderDate
                        {
                            DateReminder = reminderEdit.DateReminder.Value.AddYears(i),
                            IsDone = false,
                            ReminderID = reminderEdit.ID
                        });
                    }
                }
            }

            return reminderDates;
        }
    }
}
