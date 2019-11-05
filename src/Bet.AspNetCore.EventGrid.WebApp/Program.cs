using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.WebApp
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                        .ConfigureLogging(log =>
                        {
                            log.AddDebug();
                            log.AddConsole();
                        })
                        //.ConfigureKestrel(options => options.AllowSynchronousIO = false)
                        .UseStartup<Startup>();
        }
    }
}
