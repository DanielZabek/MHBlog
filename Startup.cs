using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MHBlog.Data;
using MHBlog.Models;
using MHBlog.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace MHBlog
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
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            var options = new RewriteOptions()
             .AddRedirectToHttps();

            app.UseRewriter(options);
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
                app.UseStaticFiles();
            }

            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
            routes.MapRoute(
                 name: "default",
                 template: "{controller=Home}/{action=Index}/{id?}/{searchString?}");
            routes.MapRoute(
                 name: "details",
                 template: "{id}/{title}",
                defaults: new { controller = "Home", action = "Details" });
            routes.MapRoute(
                name: "category",
                template: "kategoria/{id}/{name}",
                defaults: new { controller = "Home", action = "CategoryView" });
            routes.MapRoute(
                name: "tag",
                template: "tag/{id}/{name}",
                defaults: new { controller = "Home", action = "TagView" });
            routes.MapRoute(
            name: "contact",
            template: "kontakt",
            defaults: new { controller = "Home", action = "Contact" });
            });




            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //adding customs roles : Question 1
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            string roleName = "Admin";
            IdentityResult roleResult;

            
            
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 2
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }


            //Here you could create a super user who will maintain the web app
            var admin = new ApplicationUser
            {
                UserName = Configuration["AppSettings:AdminUserEmail"],
                Email = Configuration["AppSettings:AdminUserEmail"],
            };
           
            string userPWD = Configuration["AppSettings:UserPassword"];
            var _user = await UserManager.FindByEmailAsync(Configuration["AppSettings:AdminUserEmail"]);
           
            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(admin, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role : Question 3
                    await UserManager.AddToRoleAsync(admin, "Admin");

                }
            }
        } 
    }
}
