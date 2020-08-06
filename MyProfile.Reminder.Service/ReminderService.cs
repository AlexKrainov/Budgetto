using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
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

        public ReminderService(IBaseRepository baseRepository)
        {
            this.repository = baseRepository;
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
            var currentInfoID = UserInfo.Current.ID;
            var now = DateTime.Now.ToUniversalTime();



            try
            {
                if (reminderEdit.ID > 0)//Update
                {
                    var reminder = await repository.GetAll<Reminder>(x => x.ID == reminderEdit.ID).FirstOrDefaultAsync();

                    reminder.Title = reminderEdit.Title;
                    reminder.Description = reminderEdit.Description;
                    reminder.DateReminder = reminderEdit.DateReminder;
                    reminderEdit.DateEdit = reminder.DateEdit = now;
                    reminder.IsRepeat = reminderEdit.IsRepeat;
                    reminder.RepeatEvery = reminderEdit.RepeatEvery;
                    reminder.CssIcon = reminderEdit.CssIcon ?? "pe-7s-bell";

                    //ToDo ReminderDates

                    await repository.UpdateAsync(reminder, true);
                }
                else
                {
                    var reminder = new Reminder();

                    reminder.UserID = currentInfoID;
                    reminderEdit.DateEdit = reminderEdit.DateCreate = reminder.DateEdit = reminder.DateCreate = now;

                    reminder.Title = reminderEdit.Title;
                    reminder.Description = reminderEdit.Description;
                    reminder.DateReminder = reminderEdit.DateReminder;
                    reminder.IsRepeat = reminderEdit.IsRepeat;
                    reminder.RepeatEvery = reminderEdit.RepeatEvery;
                    reminder.CssIcon = reminderEdit.CssIcon ?? "pe-7s-bell";

                    //ToDo ReminderDates
                    await repository.CreateAsync(reminder, true);

                    reminderEdit.ID = reminder.ID;

                }
            }
            catch (Exception ex)
            {
                return false;

            }

            return true;
        }
    }
}
