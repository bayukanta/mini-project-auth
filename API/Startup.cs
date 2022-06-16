using BLL;
using External;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.EntityFramework.Stores;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using DAL;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using DAL.Config;

namespace API
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

            X509Certificate2 cert = new X509Certificate2("example.pfx", Configuration.GetValue<string>("Certificate:Password"));
            string migrationsAssembly = "DAL";

            IdentityModelEventSource.ShowPII = true;
            services.AddIdentityServer(options =>
            {
                options.Authentication.CookieAuthenticationScheme = "none";
                options.IssuerUri = Configuration.GetValue<string>("AuthorizationServer:Address");
            })
            .AddSigningCredential(cert)
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidatorService>()
            .AddProfileService<UserProfileService>()
            .AddProfileService<UserProfileService>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection"),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection"),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600;
            })
            .AddPersistedGrantStore<PersistedGrantStore>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration.GetValue<string>("AuthorizationServer:Address");
                options.Audience = Configuration.GetValue<string>("Service:Name");
                options.RequireHttpsMetadata = false;
            })
            .AddCookie("none");
            services.AddControllers().AddNewtonsoftJson(x =>
 x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddDbContext<MiniProjectAuthDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc();
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                     builder => builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                         .WithOrigins("*")
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .Build());
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
            services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();
            services.AddSingleton<GoogleApi>();
            services.AddScoped<IdentityServerApi>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample Auth Service", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement(){
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        },
                        new List<string>()
                        }
                });
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.Always
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample v1");
            });

            //initialize identity server, firsttime only after creating migration for ConfigurationDb & PersistedGrantDb
            //InitConfig.InitializeDatabase(app);
            //SeedData.EnsureSeedData(Configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
