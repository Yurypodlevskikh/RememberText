using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
using RememberText.RTTools.Filters;

namespace RememberText
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
            services.AddDbContext<RememberTextDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<RememberTextDBInitializer>();
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentity<User, Role>()
               .AddEntityFrameworkStores<RememberTextDbContext>()
               .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options => { options.SignIn.RequireConfirmedAccount = true; });
            services.AddTransient<IEmailSender, EmailService>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddScoped<FirstIpAddressFilter>();
            services.AddScoped<IIpAddressesData, SqlIpAddressesData>();
            services.AddScoped<IRTLanguageService, SqlLanguageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RememberTextDBInitializer db)
        {
            db.Initialize();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "PractiseSynchronous",
                //    pattern: "{controller=Home}/{action=Index}/{id?}/{index}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=IpAddresses}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
