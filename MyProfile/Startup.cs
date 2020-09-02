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
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.File.Service;
using MyProfile.Goal.Service;
using MyProfile.HelpCenter.Service;
using MyProfile.Limit.Service;
using MyProfile.LittleDictionaries.Service;
using MyProfile.Reminder.Service;
using MyProfile.Template.Service;
using MyProfile.ToDoList.Service;
using MyProfile.User.Service;
using MyProfile.User.Service.PasswordWorker;

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

            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<BaseRepository>();

            #endregion

            #region Services
            services.AddScoped<TemplateService>();
            services.AddScoped<BudgetService>();
            services.AddScoped<BudgetRecordService>();
            services.AddScoped<BudgetTotalService>();
            services.AddScoped<DictionariesService>();
            services.AddScoped<SectionService>();
            services.AddScoped<UserLogService>();
            services.AddScoped<CollectionUserService>();
            services.AddScoped<LimitService>();
            services.AddScoped<GoalService>();
            services.AddScoped<ChartService>();
            services.AddScoped<UserService>();
            services.AddScoped<FileWorkerService>();
            services.AddScoped<PasswordService>();
            services.AddScoped<ChatService>();
            services.AddScoped<FeedbackService>();
            services.AddScoped<ReminderService>();
            services.AddScoped<ToDoListService>();
            services.AddScoped<HelpCenterService>();

            services.AddScoped<UserEmailService>();
            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#if true
            string connection = Configuration.GetConnectionString("TestConnection");//TestConnection/TestRegRuConnection
#else
            string connection = Configuration.GetConnectionString("PublishConnection");

#endif
            services.AddMemoryCache();

            services.AddDbContext<MyProfile_DBContext>(options =>
                options.UseLazyLoadingProxies()
                .UseSqlServer(connection));

            //services.AddScoped<DbContext, MyProfile_DBContext>(f =>
            //{
            //    return f.GetService<MyProfile_DBContext>();
            //});

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #region Cookies settings
            services.Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Identity/Account/Login");
                });
            #endregion

            #region Email

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.AddSingleton<IEmailSender, EmailSender>();
            #endregion

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);
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
                }
                if (context.Response.StatusCode == 500)
                {
                    context.Request.Path = "/Error/Error_500";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            Identity.UserInfo.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Account}/{action=Login}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Budget}/{action=Month}/{id?}");
            });

            DBContextStorage.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            // Identity
            //ApplicationUserInfo.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            //CacheStorage.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }
    }
}
