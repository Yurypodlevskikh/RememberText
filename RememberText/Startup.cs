using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RememberText.DAL.Context;
using RememberText.Data;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Services.InEmail;
using RememberText.Infrastructure.Services.InSQL;
using RememberText.Infrastructure.Services.OnNetwork;
using RememberText.RTTools.Filters;
using Serilog;
using System;
using System.Globalization;

namespace RememberText
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration Configuration) => this.Configuration = Configuration;


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            services.AddDbContext<RememberTextDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<RememberTextDBInitializer>();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddIdentity<User, Role>()
               .AddEntityFrameworkStores<RememberTextDbContext>()
               .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options => { options.SignIn.RequireConfirmedAccount = true; });
            services.ConfigureApplicationCookie(options => 
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.LoginPath = "/Identity/Account/Login";
            });
            services.AddTransient<IEmailSender, EmailService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromDays(1); });

            services.AddControllersWithViews()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
                })
                .AddViewLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("en-US"),
                    new CultureInfo("ru-RU"),
                    new CultureInfo("sv-SE"),
                    new CultureInfo("uk-UA")
                };

                options.DefaultRequestCulture = new RequestCulture("en-GB");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddRazorPages();

            services.AddScoped<IRTTopicService, SqlTopicService>();
            services.AddScoped<IRTTextService, SqlTextService>();
            services.AddScoped<IRTNormalizedTagService, SqlNormalizedTagService>();
            services.AddScoped<IRTTagService, SqlTagService>();
            services.AddScoped<IRTTagAssignmentService, SqlTagAssignmentService>();
            services.AddScoped<IRTGuestBookService, SqlGuestBookService>();
            services.AddScoped<FirstIpAddressFilter>();
            services.AddScoped<IIpAddressesData, SqlIpAddressesData>();
            services.AddScoped<IRTLanguageService, SqlLanguageService>();
            services.AddScoped<IRTVisitorInfoData, NetworkVisitorInfoData>();
            services.AddScoped<IRTVisitService, SqlVisitService>();
            services.AddScoped<IRTTextCopyrightService, SqlTextCopyrightService>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RememberTextDBInitializer db)
        {
            db.Initialize();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                //app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseSession();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");

                endpoints.MapRazorPages();
            });
        }
    }
}
