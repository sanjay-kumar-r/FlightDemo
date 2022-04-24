using CommonUtils.Exceptions;
using CommonUtils.Filters;
using Flight.Users.Model.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceContracts.Bookings;
using ServiceContracts.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            services.AddControllers();
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
