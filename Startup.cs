using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagment.Models;
using EmployeeManagment.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace EmployeeManagment
{
    public class Startup
    {
        private  IConfiguration _config;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        //reset token
        
        public void ConfigureServices(IServiceCollection services)
        {
            //reset token
            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromHours(12);
            });
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            //external logins
            object p = services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = "489378341198-u8qdvprmsf561qi5v3bnegivl2dviu9n.apps.googleusercontent.com";
                options.ClientSecret = "jQ_8ssLuLqAeIKAgqZwLvWKp";
            })
            .AddFacebook(options=>
            {
                options.ClientId = "268908334166196";
                options.ClientSecret = "d722f008c3a8622cae6905375d0ba0ca";
            });


            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("EmployeeDbConnection"));
            }
            );
        
           
     services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            ;
            //services.Configure<PasswordOptions>();
            services.AddAuthorization(options =>
            {
                //options.AddPolicy("DeletePolicy", policy => policy.RequireClaim("Delete Role"));//.RequireClaim("Create Role"));
                //options.AddPolicy("EditPolicy", policy => policy.RequireClaim("Edit Role","true"));
                //options.AddPolicy("CreatePolicy", policy => policy.RequireClaim("Create Role", "true"));
                /*Authorazation by Claims Based on Roles  
                Func Delegete  to achive the or Option 
                */
                //options.AddPolicy("EditPolicy", policy=>policy.RequireAssertion(context=>

                //    context.User.IsInRole("Admin") &&
                //    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                //    context.User.IsInRole("Super Admin")
                //));
                //custom Authorization Handler 
                options.AddPolicy("EditPolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClamisRequirements()));

                //options.AddPolicy("AdminRolePolicy",policy=>policy.RequireRole("Admin"));
            });
            services.ConfigureApplicationCookie(options=>
            {
                options.AccessDeniedPath = new PathString("/Adminstration/AccessDenied");
            });
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsRequirement>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();



        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "MVC1005:Cannot use UseMvc with Endpoint Routing.", Justification = "<Pending>")]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                //DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions()
                //{
                //    SourceCodeLineCount = 10
                //};
                app.UseDeveloperExceptionPage();
            }
              else
            {
                app.UseExceptionHandler("/Error");

                app.UseStatusCodePagesWithReExecute("/Error/{0}");

            }





            /* FileServerOptions fileServer = new FileServerOptions();
             fileServer.DefaultFilesOptions.DefaultFileNames.Clear();
             fileServer.DefaultFilesOptions.DefaultFileNames.Add("Custom1.html");
             */
            //app.UseFileServer();
            app.UseStaticFiles();
            //app.UseRouting();


            //app.Use(async (context,next) => {
            //    logger.LogInformation("MW1:Incomming Request");
            //     await next();
            //    logger.LogInformation("MW1:Outcomming Response");
            //}) ;
            //app.Use(async (context, next) => {
            //    logger.LogInformation("MW2:Incomming Request");
            //    await next();
            //    logger.LogInformation("MW2:Outcomming Response");
            //});
            //app.Use(async (context, next) => {
            //    logger.LogInformation("MW3:Incomming Request");
            //    await next();
            //    logger.LogInformation("MW3:Outcomming Response");
            //});
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        throw new Exception("some errors accured During Run Projct");
            //        await context.Response.WriteAsync(_config["MyKey"]+" process Name :  "+ System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            //    });
            //});
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvcWithDefaultRoute();
            
            //app.UseMvc(route=>
            //{
            //    route.MapRoute("default","{countroller=Home}/{action=Index}/{id?}");
            //});
            //app.Run(async context =>
            //{
            //    //throw new Exception("There are some errors accured");
            //    await context.Response.WriteAsync("Host Environment:" + env.EnvironmentName);
            //}
            //);
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
           // app.UseMvc();



        }
    }
}
