using Email.Service.EmailEnvironment;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Threading.Tasks;

namespace Email.Service
{
    public class UserConfirmEmailService
    {
        private IBaseRepository _repository;
        private IEmailSender _emailSender;

        public UserConfirmEmailService(IBaseRepository repository,
            IEmailSender emailSender)
        {
            this._repository = repository;
            this._emailSender = emailSender;

        }

        public async Task<int> ConfirmEmail(bool isNewUser)
        {
            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                MailTypeID = (int)MailTypeEnum.ConfirmEmail,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = UserInfo.Current.ID,
            };

            try
            {

                await _emailSender.SendEmailAsync(UserInfo.Current.Email, "Test", "Test");
            }
            catch (Exception ex)
            {
                mailLog.IsSuccessful = false;
                mailLog.Comment = ex.Message;
            }

            await _repository.CreateAsync(mailLog, true);
            return 1;
        }
    }
}
