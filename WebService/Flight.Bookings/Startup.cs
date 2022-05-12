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
using ServiceContracts.Bookings;
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

namespace Flight.Bookings
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Flight.Bookings Service",
                    Description = "Project to test Flight.Bookings Service",
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
            services.AddScoped<ServiceContracts.Logger.ILogger, LoggerUtils>();
            services.AddScoped<ICustomExceptionMessageBuilder, CustomExceptionMessageBuilder>();
            services.AddScoped<GlobalAPIValidation>();
            services.AddMvc().AddMvcOptions(options => {
                options.Filters.AddService(typeof(GlobalAPIValidation));
            });
            services.AddDbContextPool<BookingsDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnectionString"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            services.AddTransient<IBookingsRepository, BookingsRepository>();

            services.AddDbContextPool<TokensDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("UsersSqlServerConnectionString"),
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
                    IEnumerable<UserRefreshTokens> refreshTokens = tokenRepo.GetRefreshTokenByFilterCondition(refreshTokenRequest);
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
                //app.UseSwagger();
                //app.UseSwaggerUI(options => {
                //    options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Flight.Bookings Service Swagger");
                //    options.RoutePrefix = string.Empty;
                //});
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Flight.Bookings Service Swagger");
                options.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
