using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagment.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using EmployeeManagment.Security;
using Microsoft.Extensions.Options;

namespace EmployeeManagment
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //This is new feature i will impliment later 
            //services.AddControllersWithViews()
            //.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            //To get connection string form appSetting.json file
            services.AddDbContextPool<AppDbContext>(
            options => options.UseSqlServer(_configuration.GetConnectionString("EmployeeDBConnection")));

            //For register Asp.Net Identity service
            //Also configer custom password rule 
            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 10;
                option.Password.RequiredUniqueChars = 3;
                option.Password.RequireNonAlphanumeric = false;

                option.SignIn.RequireConfirmedEmail = true;
                option.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            }).AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

            //OR

            //We could also do this while adding Identity services
            //For change default password rule 
            //services.Configure<IdentityOptions>(option =>
            //{
            //    option.Password.RequiredLength = 10;
            //    option.Password.RequiredUniqueChars = 3;
            //    option.Password.RequireNonAlphanumeric = false;
            //});


            //Set token life span to 5 hours this change will apply all token provider method if we want specify to a specific token generate then create to custom token provider (Default 1 day )
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(5));


            //custom token provider for a specific task
            // Changes token lifespan of just the Email Confirmation Token type

            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3));

            //For add mvc architecture
            services.AddMvc(option => option.EnableEndpointRouting = false)
                //Adding Authorization globaly
                .AddMvcOptions(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser().Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddMvcOptions(x => x.SuppressAsyncSuffixInActionNames = false)
                .AddRazorRuntimeCompilation()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            //For External login exm.- Google, Facebook
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "306183282360-r10dagna5rn28f8trp6h5rugtd7kgkod.apps.googleusercontent.com";
                    options.ClientSecret = "HLzMcjI-GMMlje1rAJANZ6cr";
                })
                .AddFacebook(option =>
                {
                    option.AppId = "3006089289497897";
                    option.AppSecret = "2e57d27dff15a82b3301abdc23150af0";
                });



            //Change Access denied path
            services.ConfigureApplicationCookie(option =>
            {
                option.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });


            services.AddAuthorization(options =>
            {
                //Simple claim Authorization policy example
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));


                //Custom Authorization policy example
                //In first condition we check user must be admin or edit role 
                // If user not in admin or edit role then we check user have super admin policy
                //We can use this way 
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(
                //    context=>context.User.IsInRole("Admin") && 
                //    context.User.HasClaim(claim=>claim.Type=="Edit Role" && claim.Value=="true") || 
                //    context.User.IsInRole("Super Admin")));


                //OR

                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));


                //Simple claim Authorization policy example
                options.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role", "true"));

                //Simple role Authorization policy example
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin", "Super Admin"));
            });

            //To set Scope of page life cycle 
            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();

            //Register the first custom authorization handler
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();

            //Register the secod custom authorization handler
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Index",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //This is new routing feature i will impliment later
            //app.UseRouting();

            //To define new mapRoute style with end points 
            //This is new routing feature i will impliment later learn and impliment later
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "Index",
            //        pattern: "{controller=Home}/{action=Index}/{id?}"
            //        );
            //});
        }
    }
}
