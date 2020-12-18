using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Tag.Service
{
    using RecordTag = MyProfile.Entity.ModelView.RecordTag;
    using DBRecordTag = MyProfile.Entity.Model.RecordTag;
    public class TagService
    {
        private IBaseRepository repository;
        private IMemoryCache cache;

        public TagService(IBaseRepository repository, IMemoryCache cache)
        {
            this.repository = repository;
            this.cache = cache;
        }


        public async Task<string> ParseAndCreateDescription(string description, List<RecordTag> recordTags, List<RecordTag> newTags)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            string newDescription = string.Empty;

            #region Create new user tags
            List<UserTag> newUserTags = new List<UserTag>();

            if (recordTags.Any(x => x.IsNew))
            {
                foreach (var recordTag in recordTags.Where(x => x.IsNew))
                {
                    if (newTags.Any(x => x.Title == recordTag.Title && x.IsNew == false))
                    {
                        recordTag.ID = newTags.FirstOrDefault(x => x.Title == recordTag.Title && x.IsNew == false).ID;
                        continue;
                    }
                    newUserTags.Add(new UserTag
                    {
                        UserID = currentUser.ID,
                        DateCreate = now,
                        Title = recordTag.Title,
                    });
                }

                if (newUserTags.Count() > 0)
                {
                    await CreateTags(newUserTags);
                }

                foreach (var newUserTag in newUserTags)
                {
                    foreach (var recordTag in recordTags)
                    {
                        if (newUserTag.Title == recordTag.Title)
                        {
                            recordTag.ID = newUserTag.ID;
                            newTags.Add(new RecordTag
                            {
                                ID = newUserTag.ID,
                                Title = newUserTag.Title,
                                Value = newUserTag.Title,
                            });
                        }
                    }
                }
                newUserTags = new List<UserTag>();
            }
            #endregion

            if (recordTags.Any(x => x.ToBeEdit))
            {
                await UpdateTags(recordTags.Where(x => x.ToBeEdit));
            }

            #region Parse description

            for (int i = 0; i < recordTags.Count; i++)
            {
                int startIndex = description.IndexOf("[[{");
                newDescription += description.Substring(0, startIndex);
                newDescription += "{{" + recordTags[i].ID + "}}";

                description = description.Substring(startIndex + 2, description.Length - startIndex - 2);
                int finishIndex = description.IndexOf("}]]");
                description = description.Substring(finishIndex + 3, description.Length - finishIndex - 3);
            }
            newDescription += description;

            #endregion

            return newDescription;
        }


        public async Task<int> CreateTags(List<UserTag> userTags)
        {
            cache.Remove(typeof(RecordTag).Name + "_" + UserInfo.Current.ID);
            return await repository.CreateRangeAsync(userTags, true);
        }

        public async Task<int> UpdateTags(IEnumerable<RecordTag> userTags)
        {
            Guid currentUserID = UserInfo.Current.ID;
            var userTagIDs = userTags.Select(x => x.ID);

            var userTagsForUpdate = await repository.GetAll<UserTag>(x => userTagIDs.Contains(x.ID) && x.UserID == currentUserID)
                 .ToListAsync();

            foreach (var userTag in userTagsForUpdate)
            {
                userTag.Title = userTags.FirstOrDefault(x => x.ID == userTag.ID).Title;
            }

            cache.Remove(typeof(RecordTag).Name + "_" + currentUserID);
            return await repository.SaveAsync();
        }

        public IList<RecordTag> GetUserTags()
        {
            var currentUserID = UserInfo.Current.ID;
            List<RecordTag> tags;

            if (cache.TryGetValue(typeof(RecordTag).Name + "_" + currentUserID, out tags) == false)
            {
                tags = repository.GetAll<UserTag>(x => x.UserID == currentUserID && x.IsDeleted == false)
                .Select(x => new RecordTag
                {
                    ID = x.ID,
                    Title = x.Title,
                    Value = x.Title,
                    DateCreate = x.DateCreate,
                    IconCss = x.IconCss,
                    Image = x.Image
                })
                .ToList();

                cache.Set(typeof(RecordTag).Name + "_" + currentUserID, tags, DateTime.Now.AddDays(1));
            }
            return tags;
        }
    }
}
