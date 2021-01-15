using Common.Service;
using Email.Service;
using Email.Service.EmailEnvironment;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProfile.Budget.Service;
using MyProfile.Chart.Service;
using MyProfile.Chat.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.File.Service;
using MyProfile.Goal.Service;
using MyProfile.HelpCenter.Service;
using MyProfile.Limit.Service;
using MyProfile.Payment.Service;
using MyProfile.Reminder.Service;
using MyProfile.Template.Service;
using MyProfile.ToDoList.Service;
using MyProfile.User.Service;
using MyProfile.User.Service.PasswordWorker;
using MyProfile.UserLog.Service;

namespace MyProfile.LandingPage
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

            #region Email

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.AddSingleton<IEmailSender, EmailSender>();
            #endregion

            #region Services
            services.AddTransient<TemplateService>();
            services.AddTransient<BudgetService>();
            services.AddTransient<BudgetRecordService>();
            services.AddTransient<BudgetTotalService>();
            services.AddTransient<SectionService>();
            services.AddTransient<UserLogService>();
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

            services.AddTransient<UserEmailService>();
            services.AddTransient<CommonService>();
            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#if false
            //string connection = Configuration.GetConnectionString("TestConnection");
            string connection = Configuration.GetConnectionString("TestRegRuConnection");
#else
            string connection = Configuration.GetConnectionString("PROD_Connection");
#endif

            services.AddMemoryCache();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = System.TimeSpan.FromDays(2);
                });

            services.AddDbContext<MyProfile_DBContext>(options =>
                options.UseLazyLoadingProxies()
                .UseSqlServer(connection));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            Identity.UserInfo.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }
    }
}
