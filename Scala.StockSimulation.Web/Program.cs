using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scala.StockSimulation.Core.Entities;
using Scala.StockSimulation.Web.Data;
using Scala.StockSimulation.Web.Services;
using Scala.StockSimulation.Web.Services.Interfaces;

namespace Scala.StockSimulation.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("ApplicationDbContext")
            ));
            builder.Services.AddScoped<FormHelperService>();
            builder.Services.AddScoped<OrderCleanupService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ExcelExportService>();

            //add identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                //configure options for testing purposes
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
             .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();
            // Add services to the container.
            builder.Services.ConfigureApplicationCookie(options =>
                options.AccessDeniedPath = "/account/login"
            );
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "adminShowStudents",
                    areaName: "Admin",
                    pattern: "Admin/ShowStudents",
                    defaults: new { controller = "Admin", action = "ShowStudents" });

                endpoints.MapAreaControllerRoute(
                    name: "adminShowResults",
                    areaName: "Admin",
                    pattern: "Admin/ShowResults",
                    defaults: new { controller = "Admin", action = "ShowResults" });

                endpoints.MapAreaControllerRoute(
                    name: "adminShowAllOrders",
                    areaName: "Admin",
                    pattern: "Admin/ShowAllOrders",
                    defaults: new { controller = "Admin", action = "ShowAllOrders" });

                endpoints.MapAreaControllerRoute(
                    name: "adminShowAllOrders",
                    areaName: "Admin",
                    pattern: "Admin/ImportStartSituation",
                    defaults: new { controller = "Admin", action = "ImportStartSituation" });

                endpoints.MapAreaControllerRoute(
                    name: "adminShowAllOrders",
                    areaName: "Admin",
                    pattern: "Admin/ShowExportOrderItems",
                    defaults: new { controller = "Admin", action = "ShowExportOrderItems" });
                endpoints.MapAreaControllerRoute(
                    name: "adminShowAllOrders",
                    areaName: "Admin",
                    pattern: "Admin/ShowOrderItems",
                    defaults: new { controller = "Admin", action = "ShowOrderItems" });

                endpoints.MapControllerRoute(
                    name: "adminGeneral",
                    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}