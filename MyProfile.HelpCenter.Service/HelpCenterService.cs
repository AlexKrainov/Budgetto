using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.HelpCenter;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.HelpCenter.Service
{
    public class HelpCenterService
    {
        private IBaseRepository repository;
        private UserLogService userLogService;

        public HelpCenterService(IBaseRepository repository)
        {
            this.repository = repository;
            userLogService = new UserLogService(repository);
        }
        //1	Test Article	1	Test Article	2020-08-31 11:00:48.4699363	2020-08-31 11:00:48.4699363	~/Areas/Help/Views/Articles/TestArticle.cshtml	NULL	Test Article

        public async Task<ArticleModelView> GetFullArticleByID(int id)
        {
            //
            return await repository.GetAll<HelpArticle>(x => x.IsVisible)
                 .Select(x => new ArticleModelView
                 {
                     ID = x.ID,
                     Link = x.Link,
                     Title = x.Title,
                    // AreaName = x.AreaName,
                     DateCreate = x.DateCreate,
                     DateEdit = x.DateEdit,
                     //Description = x.Description,
                     UserName = x.User != null ? x.User.Name + " " + x.User.LastName : null,
                     ImageLink = x.User != null ? x.User.ImageLink : null,
                     Views = x.HelpArticleUserViews.Count
                 })
                 .FirstOrDefaultAsync();
        }

        public async Task<List<ArticleModelView>> GetRelatedArticlesByID(int id)
        {
            //ToDo add algorithm find related articles
            return await repository.GetAll<HelpArticle>(x => x.IsVisible)
               .OrderBy(x => x.HelpArticleUserViews.Count)
               .Take(5)
               .Select(x => new ArticleModelView
               {
                   ID = x.ID,
                   Title = x.Title,
                   DateCreate = x.DateCreate,
                   DateEdit = x.DateEdit,
                   UserName = x.User != null ? x.User.Name + " " + x.User.LastName : null,
                   ImageLink = x.User != null ? x.User.ImageLink : null,
                   Views = x.HelpArticleUserViews.Count
               })
               .ToListAsync();
        }

        public async Task<List<ArticleModelView>> GetTopArticles()
        {
            return await repository.GetAll<HelpArticle>(x => x.IsVisible)
                .OrderBy(x => x.HelpArticleUserViews.Count)
                .Take(5)
                .Select(x => new ArticleModelView
                {
                    ID = x.ID,
                    Title = x.Title,
                    DateCreate = x.DateCreate,
                    DateEdit = x.DateEdit,
                    UserName = x.User != null ? x.User.Name + " " + x.User.LastName : null,
                    ImageLink = x.User != null ? x.User.ImageLink : null,
                    Views = x.HelpArticleUserViews.Count
                })
                .ToListAsync();
        }

        public async Task<ArticleModelView> GetArticleByID(int id)
        {
            var currentUser = UserInfo.Current;
            try
            {
                await repository.CreateAsync(new HelpArticleUserView
                {
                    DateView = DateTime.Now.ToUniversalTime(),
                    UserID = currentUser.ID,
                    HelpArticleID = id,

                }, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.HelpCenter_Article_Page);

            }
            catch (Exception ex)
            {

            }

            return await repository.GetAll<HelpArticle>(x => x.IsVisible && x.ID == id)
                 .Select(x => new ArticleModelView
                 {
                     ID = x.ID,
                     Link = x.Link,
                     Title = x.Title,
                     //AreaName = x.AreaName,
                     DateCreate = x.DateCreate,
                     DateEdit = x.DateEdit,
                     //Description = x.Description,
                     UserName = x.User != null ? x.User.Name + " " + x.User.LastName : null,
                     ImageLink = x.User != null ? x.User.ImageLink : null,
                     Views = x.HelpArticleUserViews.Count
                 })
                 .FirstOrDefaultAsync();
        }


    }
}
