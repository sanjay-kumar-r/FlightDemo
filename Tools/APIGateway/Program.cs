using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace APIGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.ConfigureAppConfiguration((host, config) =>
                //{
                //    var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                //    //config.SetBasePath(rootPath + Path.DirectorySeparatorChar + "OcelotConfigs");
                //    var ocelotPath = rootPath + Path.DirectorySeparatorChar + "OcelotConfigs";
                //    if (File.Exists(ocelotPath + Path.DirectorySeparatorChar + $"ocelot.{host.HostingEnvironment.EnvironmentName}.json"))
                //        config.AddJsonFile(ocelotPath + Path.DirectorySeparatorChar + $"ocelot.{host.HostingEnvironment.EnvironmentName}.json",
                //            optional: false, reloadOnChange: true);
                //    else if (File.Exists(ocelotPath + Path.DirectorySeparatorChar + "ocelot.json"))
                //        config.AddJsonFile(ocelotPath + Path.DirectorySeparatorChar + "ocelot.json", optional: false, reloadOnChange: true);
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .UseUrls();
                });
    }
}
