using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Payment;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Payment.Service
{
    using Payment = Entity.Model.Payment;
    public class PaymentService
    {
        private IBaseRepository repository;
        private UserLogService userLogService;

        public PaymentService(IBaseRepository repository,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
        }


        public async Task<Guid> CreatePaymentHistory(PaymentViewModel model)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            var payment = new PaymentHistory
            {
                DateClickToPay = now,
                PaymentID = currentUser.Payment.ID,
                PaymentTariffID = (int)model.TariffTypeID,
            };

            try
            {
                await repository.CreateAsync(payment, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.PaymentHistory_Create);
            }
            catch (Exception ex)
            {
                await userLogService.CreateErrorLogAsync(userSessionID: currentUser.UserSessionID, where: "PaymentService.CreatePaymentHistory", ex);
            }


            return payment.ID;
        }

        public async Task<long> Paid(Guid paymentHistoryID)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            var paymentHistory = await repository.GetAll<PaymentHistory>(x => x.ID == paymentHistoryID).FirstOrDefaultAsync();


            try
            {
                paymentHistory.IsPaid = true;
                paymentHistory.DateFinisthToPay = now;
                paymentHistory.Payment.IsPaid = true;
                paymentHistory.Payment.LastDatePayment = now;

                if (paymentHistory.PaymentTariffID == (int)PaymentTariffTypes.Standard)
                {
                    DateTime newDateTo = now > paymentHistory.Payment.DateTo ?
                        now.AddYears(1) :
                        paymentHistory.Payment.DateTo.AddYears(1);

                    paymentHistory.DateFrom = paymentHistory.Payment.DateTo;
                    paymentHistory.Payment.DateFrom = now.AddDays(-1);
                    paymentHistory.Payment.DateTo = newDateTo;
                    paymentHistory.Payment.PaymentTariffID = (int)PaymentTariffTypes.Standard;
                    paymentHistory.DateTo = newDateTo;
                }

                await repository.UpdateAsync(paymentHistory, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.PaymentHistory_Update);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Payment_Update);

                currentUser.IsAvailable = paymentHistory.Payment.DateFrom <= now && paymentHistory.Payment.DateTo >= now;
                currentUser.Payment = new Payment
                {
                    DateFrom = paymentHistory.Payment.DateFrom,
                    DateTo = paymentHistory.Payment.DateTo,
                    ID = paymentHistory.Payment.ID,
                    IsPaid = paymentHistory.Payment.IsPaid,
                    LastDatePayment = paymentHistory.Payment.LastDatePayment,
                    PaymentTariffID = paymentHistory.Payment.PaymentTariffID,
                };
                await UserInfo.AddOrUpdate_Authenticate(currentUser);

            }
            catch (Exception ex)
            {
                await userLogService.CreateErrorLogAsync(userSessionID: currentUser.UserSessionID, where: "PaymentService.Paid", ex);

                return -1;
            }

            return paymentHistory.PaymentID;
        }
    }
}
