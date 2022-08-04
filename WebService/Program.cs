using NLog;
using NLog.Web;
using NLog.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace WebService
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).
            ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddDebug();
                logging.AddNLog();
            }).ConfigureWebHostDefaults(webBuilder =>
            {
                //webBuilder.UseStartup<StartupBase>();
            });
        }
    }
}