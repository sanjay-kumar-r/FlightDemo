using CommonUtils.Exceptions;
using CommonUtils.Filters;
using CommonUtils.Swagger;
using Flight.Users.Model.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiceContracts;
using ServiceContracts.CustomException;
using ServiceContracts.Users;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UsersDTOs;
using UsersRepositoryUtils.DBContext;
using UsersRepositoryUtils.DBHelper;
using Utils.Logger;

namespace Flight.Users
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Flight.Users Service",
                    Description = "Project to test Flight.Users Service",
                    Version = "1.0"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "This site uses Bearer token and you have to pass it as : Bearer<space>Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                            Scheme="oauth2",
                            Name="Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.OperationFilter<CustomHeaderSwaggerAttribute>();

                var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                options.IncludeXmlComments(filePath);
            });

            //logger
            services.AddScoped<ServiceContracts.Logger.ILogger, LoggerUtils>();

            //global filter
            services.AddScoped<ICustomExceptionMessageBuilder, CustomExceptionMessageBuilder>();
            services.AddScoped<GlobalAPIValidation>();
            services.AddMvc().AddMvcOptions(options => {
                options.Filters.AddService(typeof(GlobalAPIValidation));
            });

            //db context
            services.AddDbContextPool<UsersDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnectionString"),
                b => b.MigrationsAssembly("UsersRepositoryUtils")));
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddDbContextPool<TokensDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnectionString"),
                b => b.MigrationsAssembly("UsersRepositoryUtils")));
            services.AddTransient<ITokensRepository, TokensRepository>();

            //auth
            var authKey = Configuration.GetValue<string>("AuthSettings:Key");
            var keyBytes = Encoding.ASCII.GetBytes(authKey ?? string.Empty);
            TokenValidationParameters tokenValidation = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            };
            services.AddSingleton(tokenValidation);
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = tokenValidation;
                jwtOptions.Events = new JwtBearerEvents();
                jwtOptions.Events.OnTokenValidated = async (context) =>
                {
                    var ipAddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    var jwtToken = context.SecurityToken as JwtSecurityToken;
                    var tokenRepo = context.Request.HttpContext.RequestServices.GetService<ITokensRepository>();
                    var refreshTokenRequest = new UserRefreshTokens
                    {
                        Token = jwtToken.RawData,
                        IpAddress = ipAddress
                    };
                    var refreshTokens = tokenRepo.GetRefreshTokenByFilterCondition(refreshTokenRequest);
                    if (refreshTokens == null || refreshTokens.Count() <= 0 || refreshTokens.FirstOrDefault() == null)
                    {
                        context.Fail("Invalid Token Details");
                    }
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                //app.UseSwagger(options =>
                //{
                //    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                //});
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Flight.Users Service Swagger");
                    //options.RoutePrefix = "swagger";
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
