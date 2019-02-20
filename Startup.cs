using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nexzoz.API.Data;
using Nexzoz.API.Models;
using Nexzoz.API.Helpers;
using Nexzoz.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Nexzoz.API.Data.Interfaces;

namespace Nexzoz.API
{
    public class Startup
    {
        #region Properties

        #region Internals
        IConfiguration Configuration { get; }
        SymmetricSecurityKey SecretKey { get; }
        #endregion

        #endregion

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SecretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("Authentication:Key").Value));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            
            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddScoped<IAuthRepository, AuthRepository>();


            services.AddAutoMapper();
            services.AddDbContext<UserDataContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Configure JwtIssuerOptions
            JwtIssuerOptions jwtIssuerOptions = Configuration.GetSection(nameof(JwtIssuerOptions)).Get<JwtIssuerOptions>();
            services.Configure<JwtIssuerOptions>(options =>
            {
                options = jwtIssuerOptions;
                options.SigningCredentials = new SigningCredentials(SecretKey, SecurityAlgorithms.HmacSha512Signature);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtIssuerOptions.Issuer;
                configureOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuerOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtIssuerOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecretKey,

                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                configureOptions.SaveToken = true;
            });

            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            var identityBuilder = services.AddIdentityCore<NexzozUser>(u =>
            {
                u.Password.RequireDigit = false;
                u.Password.RequireLowercase = false;
                u.Password.RequireUppercase = false;
                u.Password.RequireNonAlphanumeric = false;
                u.Password.RequiredLength = 8;
                u.User.AllowedUserNameCharacters = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //u.ClaimsIdentity.UserNameClaimType = 
            });
            //identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
            identityBuilder.AddEntityFrameworkStores<UserDataContext>().AddDefaultTokenProviders();

            /*
            services.AddDbContext<UserDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("AngularASPNETCore2WebApiAuth"))); */
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
