using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.SubScription;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MyProfile.SubScription.Service
{
    public class SubScriptionService
    {
        private BaseRepository repository;
        private UserLogService userLogService;

        public SubScriptionService(BaseRepository repository,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
        }


        public List<BaseSubScriptionModelView> GetBaseSubScriptions()
        {
            var subScriptions = repository.GetAll<MyProfile.Entity.Model.SubScription>(x => x.IsActive)
                  .Select(x => new BaseSubScriptionModelView
                  {
                      ID = x.ID,
                      LogoBig = x.LogoBig,
                      Site = x.Site,
                      CategoryName = x.SubScriptionCategory.Title,
                      Title = x.Title,
                      Options = x.SubScriptionOptions
                          .Where(y => y.IsActive)
                          .OrderBy(y => y.IsPersonally)
                          .Select(y => new SubScriptionOptionModelView
                          {
                              ID = y.ID,
                              IsSelected = false,
                              IsBoth = y.IsBoth,
                              IsFamaly = y.IsFamaly,
                              IsPersonally = y.IsPersonally,
                              IsStudent = y.IsStudent,
                              Title = y.Title, // ?? x.Title,
                              Pricings = y.SubScriptionPricings
                                .OrderBy(z => z.PricingPeriodTypeID)
                                .Select(z => new SubScriptionPricingModelView
                                {
                                    ID = z.ID,
                                    Price = z.Price,
                                    PricePerMonth = z.PricePerMonth,
                                    PeriodTypeID = z.PricingPeriodTypeID,
                                })
                                .ToList()
                          })
                          .ToList()
                  })
                  .ToList();

            GetDiaposonPrice(subScriptions);

            return subScriptions;
        }

        public List<BaseSubScriptionModelView> GetUserSubScriptions()
        {
            var currentUser = UserInfo.Current;

            var subScriptions = repository.GetAll<UserSubScription>(x => x.UserID == currentUser.ID && x.IsDeleted == false)
                  .Select(x => new BaseSubScriptionModelView
                  {
                      UserPricingID = x.SubScriptionPricingID == null ? -1 : x.SubScriptionPricingID.Value,
                      UserPrice = x.Price,
                      UserPriceForMonth = x.PricePerMonth,
                      UserTitle = x.Title,
                      UserSubScriptionID = x.ID,
                      UserPricingPeriodTypeID = x.SubScriptionPricing.PricingPeriodTypeID,

                      ID = x.SubScriptionPricing.SubScriptionOption.SubScriptionID,
                      LogoBig = x.SubScriptionPricing.SubScriptionOption.SubScription.LogoBig,
                      Site = x.SubScriptionPricing.SubScriptionOption.SubScription.Site,
                      CategoryName = x.SubScriptionPricing.SubScriptionOption.SubScription.SubScriptionCategory.Title,
                      Title = x.SubScriptionPricing.SubScriptionOption.SubScription.Title,
                      Options = x.SubScriptionPricing.SubScriptionOption.SubScription.SubScriptionOptions
                            .Where(y => y.IsActive)
                            .OrderBy(y => y.IsPersonally)
                            .Select(y => new SubScriptionOptionModelView
                            {
                                ID = y.ID,
                                IsSelected = x.SubScriptionPricing.SubScriptionOptionID == y.ID,
                                IsBoth = y.IsBoth,
                                IsFamaly = y.IsFamaly,
                                IsPersonally = y.IsPersonally,
                                IsStudent = y.IsStudent,
                                Title = y.Title, // ?? x.Title,

                                Pricings = y.SubScriptionPricings
                                  .OrderBy(z => z.PricingPeriodTypeID)
                                  .Select(z => new SubScriptionPricingModelView
                                  {
                                      ID = z.ID,
                                      Price = z.Price,
                                      PricePerMonth = z.PricePerMonth,
                                      PeriodTypeID = z.PricingPeriodTypeID,
                                      IsSelected = x.SubScriptionPricingID == z.ID,
                                  })
                                  .ToList()
                            })
                            .ToList()
                  })
                  .ToList();

            GetDiaposonPrice(subScriptions);

            return subScriptions;
        }

        public void GetDiaposonPrice(List<BaseSubScriptionModelView> subScriptions)
        {
            var currentUser = UserInfo.Current;

            for (int i = 0; i < subScriptions.Count; i++)
            {
                decimal minPrice = subScriptions[i].Options.Min(x => x.Pricings.Min(y => y.Price));
                decimal maxPrice = subScriptions[i].Options.Max(x => x.Pricings.Max(y => y.Price));

                if (minPrice == maxPrice)
                {
                    subScriptions[i].DiapasonPrice = (minPrice).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));
                }
                else
                {
                    subScriptions[i].DiapasonPrice = (minPrice).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture)) + " - " + (maxPrice).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));
                }

                for (int y = 0; y < subScriptions[i].Options.Count; y++)
                {
                    subScriptions[i].Options[y].IsSelected = y == 0;
                    minPrice = subScriptions[i].Options[y].Pricings.Min(x => x.Price);
                    maxPrice = subScriptions[i].Options[y].Pricings.Max(x => x.Price);

                    if (minPrice == maxPrice)
                    {
                        subScriptions[i].Options[y].DiapasonPrice = (minPrice).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));
                    }
                    else
                    {
                        subScriptions[i].Options[y].DiapasonPrice = (minPrice).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture)) + " - " + (maxPrice).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));
                    }
                }
            }
        }

        public void CraeteOrUpdate(BaseSubScriptionModelView baseSubScriptionModelView)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            if (baseSubScriptionModelView.UserSubScriptionID == 0) //create
            {
                UserSubScription userSubScription = new UserSubScription
                {
                    CreateDate = now,
                    Price = baseSubScriptionModelView.UserPrice,
                    PricePerMonth = baseSubScriptionModelView.UserPriceForMonth,
                    Title = baseSubScriptionModelView.UserTitle,
                    UserID = currentUser.ID,
                    SubScriptionPricingID = baseSubScriptionModelView.UserPricingID
                };

                try
                {
                    repository.Create(userSubScription, true);
                    userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.SubScription_Create);
                }
                catch (Exception ex)
                {
                }

            }
            else //edit
            {

                userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.SubScription_Edit);
            }

        }

        public bool Remove(int id)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            List<long> errorLogIDs = new List<long>();
            bool result = true;

            try
            {
                var userSubScription = repository.GetAll<UserSubScription>(x => x.UserID == currentUser.ID && x.IsDeleted == false && x.ID == id)
                    .FirstOrDefault();

                userSubScription.IsDeleted = true;

                repository.Update(userSubScription, true);
            }
            catch (Exception ex)
            {
                errorLogIDs.Add(userLogService.CreateErrorLog(currentUser.UserSessionID, "SubScriptionService_Remove", ex));
                result = false;
            }

            userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.SubScription_Delete, errorLogIDs: errorLogIDs);
            return result;
        }
        public bool Recovery(int id)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            List<long> errorLogIDs = new List<long>();
            bool result = true;
            try
            {
                var userSubScription = repository.GetAll<UserSubScription>(x => x.UserID == currentUser.ID && x.IsDeleted && x.ID == id)
                    .FirstOrDefault();

                userSubScription.IsDeleted = false;

                repository.Update(userSubScription, true);
            }
            catch (Exception ex)
            {
                errorLogIDs.Add(userLogService.CreateErrorLog(currentUser.UserSessionID, "SubScriptionService_Recovery", ex));
                result = false;
            }

            userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.SubScription_Recovery, errorLogIDs: errorLogIDs);
            return result;
        }
    }
}
