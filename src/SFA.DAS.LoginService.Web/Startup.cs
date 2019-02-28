﻿using System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private ILoginConfig _loginConfig;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            WireUpDependencies(services);
            
            services.AddDbContext<LoginContext>(options => options.UseSqlServer(_loginConfig.SqlConnectionString));
            
            services.AddDbContext<LoginUserContext>(options => options.UseSqlServer(_loginConfig.SqlConnectionString));

            services.AddIdentity<LoginUser, IdentityRole>(
                    options =>
                    {
                        options.Password.RequiredLength = 8;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                    })
                .AddEntityFrameworkStores<LoginUserContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddAuthentication()
                .AddJwtBearer(jwt =>
                {
                    jwt.Authority = "http://localhost:5000";
                    jwt.RequireHttpsMetadata = false;
                    jwt.Audience = "api1";
                });
            
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(_loginConfig.SqlConnectionString);
                    options.DefaultSchema = "IdentityServer";
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(_loginConfig.SqlConnectionString);
                    options.DefaultSchema = "IdentityServer";
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<LoginUser>();
        }

        private void WireUpDependencies(IServiceCollection services)
        {
            services.AddTransient<IConfigurationService, ConfigurationService>();
            
            _loginConfig = new ConfigurationService()
                .GetLoginConfig(
                    Configuration["EnvironmentName"], 
                    Configuration["ConfigurationStorageConnectionString"],
                    "1.0",
                    "SFA.DAS.LoginService", _environment).Result;
            
            services.AddTransient(sp => _loginConfig);
            services.AddTransient<ICodeGenerationService, CodeGenerationService>();
            services.AddTransient<IHashingService, HashingService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IUserService, UserService>();
            services.AddHttpClient<ICallbackService, CallbackService>();

            services.AddMediatR(typeof(CreateInvitationHandler).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseIdentityServer();
            
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}