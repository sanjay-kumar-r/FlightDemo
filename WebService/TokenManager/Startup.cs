using CommonUtils.Exceptions;
using CommonUtils.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceContracts;
using ServiceContracts.CustomException;
using ServiceContracts.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UsersRepositoryUtils.DBContext;
using UsersRepositoryUtils.DBHelper;
using Utils.Logger;

namespace TokenManager
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
            //cors
            services.AddCors();

            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "TokenManager Service",
                    Description = "Project to test TokenManager Service",
                    Version = "1.0"
                });
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
            services.AddDbContextPool<TokensDBContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnectionString"),
                b => b.MigrationsAssembly("UsersRepositoryUtils")));
            services.AddTransient<ITokensRepository, TokensRepository>();
            services.AddDbContextPool<UsersDBContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnectionString"),
                 b => b.MigrationsAssembly("UsersRepositoryUtils")));
            services.AddTransient<IUsersRepository, UsersRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(options =>
                //{
                //    options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Token Manager Service Swagger");
                //    //options.RoutePrefix = "swagger";
                //    options.RoutePrefix = string.Empty;
                //});
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Token Manager Service Swagger");
                //options.RoutePrefix = "swagger";
                options.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
