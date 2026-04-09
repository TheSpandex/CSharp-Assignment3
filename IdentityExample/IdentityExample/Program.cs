using IdentityExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            //Setup Database Context
            builder.Services.AddDbContext<AppIdentityDBContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnStr")));

            //Setup Asp.Net Core Identity
            // Set up ASP.NET Core Identity
            builder.Services.AddIdentity<AppUser, IdentityRole>() //Turn on Identity using our custom AppUser
                            .AddEntityFrameworkStores<AppIdentityDBContext>() //save all the data in our SQL database using EF Core
                            .AddDefaultTokenProviders(); //give us the tools to reset passwords, emails, 2FA etc later.

            var app = builder.Build();

            // --- Configure Middleware Pipeline (Formerly Configure in Startup.cs) ---
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Authentication MUST come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
