using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.CollectiveViewModels;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.User.Service
{
    using User = Entity.Model.User;
    public class CollectionUserService
    {
        private IBaseRepository repository;

        public CollectionUserService(IBaseRepository repository)
        {
            this.repository = repository;
        }

        public List<Guid> GetAllCollectiveUserIDs()
        {
            var user = UserInfo.Current;
            List<Guid> allCollectiveUserIDs = new List<Guid>();

            if (UserInfo.Current.IsAllowCollectiveBudget && user.CollectiveBudgetID != Guid.Empty)
            {
                allCollectiveUserIDs.AddRange(repository.GetAll<CollectiveBudget>(x => x.ID == user.CollectiveBudgetID)
                    .SelectMany(x => x.CollectiveBudgetUsers)
                    .Where(x => x.User.IsAllowCollectiveBudget && x.Status == CollectiveUserStatusType.Accepted.ToString())
                    .Select(x => x.UserID)
                    .ToList());
            }
            else
            {
                allCollectiveUserIDs.Add(user.ID);
            }
            return allCollectiveUserIDs;
        }

        public async Task<List<Guid>> GetAllCollectiveUserIDsAsync()
        {
            var user = UserInfo.Current;
            List<Guid> allCollectiveUserIDs = new List<Guid>();

            if (UserInfo.Current.IsAllowCollectiveBudget && user.CollectiveBudgetID != Guid.Empty)
            {
                allCollectiveUserIDs.AddRange(await repository.GetAll<CollectiveBudget>(x => x.ID == user.CollectiveBudgetID)
                    .SelectMany(x => x.CollectiveBudgetUsers)
                    .Where(x => x.User.IsAllowCollectiveBudget && x.Status == CollectiveUserStatusType.Accepted.ToString())
                    .Select(x => x.UserID)
                    .ToListAsync());
            }
            else
            {
                allCollectiveUserIDs.Add(user.ID);
            }
            return allCollectiveUserIDs;
        }


        public async Task<CollectiveUserModelView> SearchUserByEmail(string email)
        {
            return await repository.GetAll<User>(x =>
                x.Email == email
                && x.IsAllowCollectiveBudget
                && x.Email != UserInfo.Current.Email)
                   .Select(x => new CollectiveUserModelView
                   {
                       Name = x.Name,
                       LastName = x.LastName,
                       Email = x.Email,
                       ImageLink = x.ImageLink
                   })
                   .FirstOrDefaultAsync();

        }

        public async Task<List<CollectiveUserModelView>> GetCollectiveUsersByCurrentUser()
        {
            var currentUser = UserInfo.Current;

            if (currentUser.CollectiveBudgetID == Guid.Empty)
            {
                return null;
            }

            return await repository.GetAll<CollectiveBudgetUser>(x =>
                        x.CollectiveBudgetID == currentUser.CollectiveBudgetID)
                     .Select(x => new CollectiveUserModelView
                     {
                         Name = x.User.Name,
                         LastName = x.User.LastName,
                         Email = x.User.Email,
                         ImageLink = x.User.ImageLink,
                         Status = x.Status,
                         DateAdded = x.DateAdded,
                         DateUpdate = x.DateUpdate,
                         CollectiveBudgetID = x.CollectiveBudgetID,
                     })
                     .ToListAsync();
        }

        public async Task<List<OfferModelView>> CheckOffers()
        {
            var currentUser = UserInfo.Current;

            var offers = await repository.GetAll<User>(x => x.ID == currentUser.ID)
                 .SelectMany(x => x.CollectiveBudgetRequests.Where(y => y.Status ==
                 Enum.GetName(typeof(RequestStatusType),
                         RequestStatusType.Awaiting) && y.IsDeleted == false))
                 .Select(x => new OfferModelView
                 {
                     OfferID = x.ID,
                     DateAdded = x.DateAdded,
                     DateUpdate = x.DateUpdate,
                     CollectiveBudgetID = x.CollectiveBudgetID,
                     OwnerName = x.CollectiveBudgetRequestOwner.User.Name,
                     OwnerLastName = x.CollectiveBudgetRequestOwner.User.LastName,
                     OwnerImageLink = x.CollectiveBudgetRequestOwner.User.ImageLink,
                     OwnerEmail = x.CollectiveBudgetRequestOwner.User.Email,
                 })
                 .ToListAsync();
            return offers;
        }

        public async Task<bool> OfferAction(int offerID, bool action)
        {
            var offer = await repository.GetByIDAsync<CollectiveBudgetRequest>(offerID);
            var now = DateTime.Now;
            var currentUser = UserInfo.Current;

            offer.DateUpdate = now;

            if (action)//it means - yes
            {
                var collectiveUser = new CollectiveBudgetUser
                {
                    CollectiveBudgetID = offer.CollectiveBudgetID,
                    UserID = currentUser.ID,
                    DateAdded = now,
                    DateUpdate = now,
                    Status = CollectiveUserStatusType.Accepted.ToString()
                };
                offer.Status = RequestStatusType.Accepted.ToString();

                var collectiveUsers = await repository.GetAll<CollectiveBudgetUser>(x => x.UserID == currentUser.ID).ToListAsync();

                if (collectiveUsers.Count >= 1)
                {
                    //Very strange
                    await repository.DeleteRangeAsync(collectiveUsers, true);

                    await repository.CreateAsync(collectiveUser, true);
                }

                currentUser.CollectiveBudgetID = offer.CollectiveBudgetID;
                await UserInfo.AddOrUpdate_Authenticate(currentUser);

            }
            else// it means - no
            {
                offer.Status = RequestStatusType.Rejected.ToString();
            }
            await repository.UpdateAsync(offer, true);

            return true;
        }

        /// <summary>
        /// if newStatus 'Gone' or 'Pouse' set to User.IsAllowCollectiveBudget = false
        /// if newStatus 'Accepted' set to User.IsAllowCollectiveBudget = true
        /// </summary>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task<bool> ChangeStatusInCollectiveBudget(string newStatus)
        {
            var currentUser = UserInfo.Current;

            if (currentUser.CollectiveBudgetID != Guid.Empty)
            {
                var collectiveBudgetUser = await repository.GetAll<CollectiveBudgetUser>
                    (x => x.UserID == currentUser.ID && x.CollectiveBudgetID == currentUser.CollectiveBudgetID)
                    .FirstOrDefaultAsync();
                collectiveBudgetUser.Status = newStatus;
                collectiveBudgetUser.DateUpdate = DateTime.Now;

                if (newStatus == CollectiveUserStatusType.Gone.ToString())
                {
                    await repository.DeleteAsync(collectiveBudgetUser, true);
                    currentUser.CollectiveBudgetID = Guid.Empty;
                    await UserInfo.AddOrUpdate_Authenticate(currentUser);
                }
                else if (newStatus == CollectiveUserStatusType.Poused.ToString())
                {
                    await Update_IsAllowCollectiveBudget(false);
                    await repository.UpdateAsync(collectiveBudgetUser, true);
                }
                else if (newStatus == Enum.GetName(typeof(CollectiveUserStatusType),
                        CollectiveUserStatusType.Accepted))
                {
                    await Update_IsAllowCollectiveBudget(true);
                    await repository.UpdateAsync(collectiveBudgetUser, true);
                }
            }
            else
            {
                //Tested it, can it be ?
                if (newStatus == CollectiveUserStatusType.Gone.ToString())
                {

                }
                else if (newStatus == CollectiveUserStatusType.Poused.ToString())
                {
                    await Update_IsAllowCollectiveBudget(false);
                }
                else if (newStatus == Enum.GetName(typeof(CollectiveUserStatusType),
                        CollectiveUserStatusType.Accepted))
                {
                    await Update_IsAllowCollectiveBudget(true);
                }
            }

            return true;
        }


        public async Task<bool> Update_IsAllowCollectiveBudget(bool isAllowCollectiveBudget)
        {
            var user = UserInfo.Current;
            var dbUser = await repository.GetAll<User>(x => x.ID == user.ID)
              .FirstOrDefaultAsync();

            user.IsAllowCollectiveBudget = dbUser.IsAllowCollectiveBudget = isAllowCollectiveBudget;

            await repository.UpdateAsync(dbUser, true);
            await UserInfo.AddOrUpdate_Authenticate(user);

            return true;
        }

        /// <summary>
        /// Get users whom the authorized user sent offer
        /// </summary>
        /// <returns></returns>
        public async Task<List<CollectiveUserModelView>> GetCollectiveRequests()
        {
            var currentUser = UserInfo.Current;

            return await repository.GetAll<CollectiveBudgetRequest>
                (x => x.CollectiveBudgetRequestOwner.UserID == currentUser.ID && x.IsDeleted == false)
                     .Select(x => new CollectiveUserModelView
                     {
                         Name = x.User.Name,
                         LastName = x.User.LastName,
                         Email = x.User.Email,
                         ImageLink = x.User.ImageLink,
                         Status = x.Status,
                         DateAdded = x.DateAdded,
                         DateUpdate = x.DateUpdate,
                         CollectiveBudgetID = x.CollectiveBudgetID,
                     })
                     .ToListAsync();
        }

        public async Task<bool> SendOffer(string email)
        {
            var requestUserID = await repository
                .GetAll<User>(x =>
                    x.Email == email
                    && x.IsAllowCollectiveBudget)
                .Select(x => x.ID)
                .FirstOrDefaultAsync();
            var currentUser = UserInfo.Current;
            var now = DateTime.Now;

            if (requestUserID == Guid.Empty)
            {
                return false; //it means the user set IsAllowCollectiveBudget = false
            }

            if (currentUser.CollectiveBudgetID == Guid.Empty)
            {
                var collectiveUser = new CollectiveBudgetUser
                {
                    CollectiveBudget = new CollectiveBudget
                    {
                        ID = Guid.NewGuid(),
                        Name = currentUser.Email
                    },
                    UserID = currentUser.ID,
                    DateAdded = now,
                    DateUpdate = now,
                    Status = CollectiveUserStatusType.Accepted.ToString()
                };

                await repository.CreateAsync(collectiveUser, true);
                currentUser.CollectiveBudgetID = collectiveUser.CollectiveBudgetID;
                await UserInfo.AddOrUpdate_Authenticate(currentUser);
            }
            //we the requestUserID did not has alredy the some offer, then send offer 
            if (await repository.AnyAsync<CollectiveBudgetRequest>(
                x => x.UserID == requestUserID
                && x.CollectiveBudgetID == currentUser.CollectiveBudgetID
                && x.Status != RequestStatusType.Rejected.ToString()
                && x.IsDeleted == false) == false)
            {
                var userRequest = new CollectiveBudgetRequest
                {
                    CollectiveBudgetID = currentUser.CollectiveBudgetID,
                    CollectiveBudgetRequestOwner = new CollectiveBudgetRequestOwner
                    {
                        UserID = currentUser.ID
                    },
                    UserID = requestUserID,
                    DateAdded = now,
                    DateUpdate = now,
                    Status = RequestStatusType.Awaiting.ToString(),
                };

                await repository.CreateAsync(userRequest, true);
                return true;
            }

            return false;
        }
    }
}
