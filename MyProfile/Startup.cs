using Common.Service;
using Email.Service;
using Email.Service.EmailEnvironment;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyProfile.Budget.Service;
using MyProfile.Chart.Service;
using MyProfile.Chat.Service;
using MyProfile.Code;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.File.Service;
using MyProfile.Goal.Service;
using MyProfile.HelpCenter.Service;
using MyProfile.Limit.Service;
using MyProfile.Payment.Service;
using MyProfile.Reminder.Service;
using MyProfile.Tag.Service;
using MyProfile.Template.Service;
using MyProfile.ToDoList.Service;
using MyProfile.User.Service;
using MyProfile.User.Service.PasswordWorker;
using MyProfile.UserLog.Service;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using MyProfile.Code.Sheduler.Shedulers;
using MyProfile.Hubs;
using MyProfile.Code.Hubs;
using MyProfile.Models;
using MyProfile.Notification.Service;
using Telegram.Service;

namespace MyProfile
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Repositories

            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<BaseRepository>();

            #endregion

            #region Services
            services.AddTransient<TemplateService>();
            services.AddTransient<BudgetService>();
            services.AddTransient<BudgetRecordService>();
            services.AddTransient<BudgetTotalService>();
            services.AddTransient<SectionService>();
            services.AddTransient<CollectionUserService>();
            services.AddTransient<LimitService>();
            services.AddTransient<GoalService>();
            services.AddTransient<ChartService>();
            services.AddTransient<UserService>();
            services.AddTransient<FileWorkerService>();
            services.AddTransient<PasswordService>();
            services.AddTransient<ChatService>();
            services.AddTransient<FeedbackService>();
            services.AddTransient<ReminderService>();
            services.AddTransient<ToDoListService>();
            services.AddTransient<HelpCenterService>();
            services.AddTransient<PaymentService>();
            services.AddTransient<TagService>();
            services.AddTransient<AccountService>();
            services.AddTransient<SummaryService>();

            services.AddTransient<UserEmailService>();
            services.AddTransient<CommonService>();
            services.AddTransient<UserLogService>();
            services.AddTransient<CurrencyService>();
            services.AddTransient<NotificationService>();
            services.AddTransient<TelegramService>();
            services.AddTransient<NotificationEmailService>();

            #endregion

            //services.AddSingleton<IUserConnectionManagerOLD, UserConnectionManagerOLD>();

            #region Task Schedulers

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<QuartzHostedService>();


            //https://www.freeformatter.com/cron-expression-generator-quartz.html
            services.AddTransient<ResetCachbackAccountTask>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(ResetCachbackAccountTask),
                 cronExpression: "0 0 01 1 * ?")); //At 12:00 AM, on day 2 of the month (0 0 1 2 * ? *)
            CronExpression.ValidateExpression("0 0 01 1 * ?");


            services.AddTransient<SetDoneReminderTask>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(SetDoneReminderTask),
                 cronExpression: "0 0 1 * * ?")); //Every day At 01:00 AM
            CronExpression.ValidateExpression("0 0 1 * * ?");

            if (PublishSettings.IsOnlyProdTask)
            {
                services.AddTransient<CurrencyHistoryTask>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(CurrencyHistoryTask),
                     cronExpression: "0 0 * ? * *")); //Every 1 hour
                CronExpression.ValidateExpression("0 0 * ? * *");
            }

            //Remove all connectionID for the day
            services.AddTransient<ResetHubConnectTask>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(ResetHubConnectTask),
                 cronExpression: "0 0 4 * * ?")); //Every day At 04:00 AM
            CronExpression.ValidateExpression("0 0 4 * * ?");

            //ToDo: сделать проверку за 30 секунд до отправке уведомления 
            services.AddTransient<CheckerLimitTask>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(CheckerLimitTask),
                 cronExpression: "0 * * ? * *")); //Every 1 minute
            CronExpression.ValidateExpression("0 * * ? * *");

            services.AddTransient<CheckerReminderTask>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(CheckerReminderTask),
                 cronExpression: "0 */5 * ? * *")); //Every 5 minuts
            CronExpression.ValidateExpression("0 */5 * ? * *");

            services.AddTransient<NotificationOnSiteTask>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(NotificationOnSiteTask),
                 cronExpression: "0/17 * * ? * * *")); //Every 17 seconds
            CronExpression.ValidateExpression("0/17 * * ? * * *");

            services.AddTransient<NotificationOnTelegramTask>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(NotificationOnTelegramTask),
                 cronExpression: "0 * * ? * *")); //Every 1 minute
            CronExpression.ValidateExpression("0 * * ? * *"); 
            
            services.AddTransient<NotificationOnMailTask>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(NotificationOnMailTask),
                 cronExpression: "0 * * ? * *")); //Every 1 minute
            CronExpression.ValidateExpression("0 * * ? * *");

            services.AddTransient<NotificationReset>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(NotificationReset),
                 cronExpression: "0 0 010 1 * ?")); //At 10:00:00am, on the 1st day, every month
            CronExpression.ValidateExpression("0 0 010 1 * ?");

            services.AddTransient<AccountDailyWork>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(AccountDailyWork),
                 cronExpression: "0 */5 * ? * *")); //At 07:00:00am every day(0 0 7 * * ?)
            CronExpression.ValidateExpression("0 0 7 * * ?");

            //"0 */5 * ? * *" - Every 5 minutes
            //0 0 1 2 * ? * - At 01:00 AM, on day 2 of the month

            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            string connection = Configuration.GetConnectionString(PublishSettings.ConnectionString);

            services.AddMemoryCache();

            services.AddDbContext<MyProfile_DBContext>(options =>
                options.UseLazyLoadingProxies()
                .UseSqlServer(connection));

            //services.AddScoped<DbContext, MyProfile_DBContext>(f =>
            //{
            //    return f.GetService<MyProfile_DBContext>();
            //});

            #region Cookies settings
            //services.Configure<CookiePolicyOptions>(options =>
            //    {
            //        options.CheckConsentNeeded = context => true;
            //        options.MinimumSameSitePolicy = SameSiteMode.None;
            //    });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Identity/Account/Login");
                    options.ExpireTimeSpan = System.TimeSpan.FromDays(2);
                });
            #endregion

            #region Email

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.AddSingleton<IEmailSender, EmailSender>();
            #endregion

            services.Configure<ProjectConfig>(Configuration.GetSection("ProjectConfig"));// In controller is using like IOptions<ProjectConfig> config

            services
                .AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ServiceActivator.Configure(app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/StatusCode");//For 500 error
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Error/Error_404";
                    await next();
                }else if (context.Response.StatusCode == 500)
                {
                    context.Request.Path = "/Error/Error_500";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //app.UseCors("CorsPolicy");

            app.UseAuthentication();

            Identity.UserInfo.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Account}/{action=Login}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Budget}/{action=Month}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationHub>("/NotificationHub");
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationUserHub>("/NotificationUserHub");
            });

            DBContextStorage.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            // Identity
            //ApplicationUserInfo.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            //CacheStorage.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var repository = serviceScope.ServiceProvider.GetRequiredService<BaseRepository>();
                var telegramService = serviceScope.ServiceProvider.GetRequiredService<TelegramService>();
                Inserters inserters = new Inserters(repository, app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>()
                    , app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

                telegramService.Start();
            }
        }
    }
}
