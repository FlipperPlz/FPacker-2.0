using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using NLog;
using NLog.Web;
using NLog.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using FPackerLibrary;

namespace WebService;

internal class Program
{
    private static readonly Mutex mutex = new Mutex(true, "-HttpServer-Mutex-");

    private static string ApplicationName => Path.GetFileNameWithoutExtension(Application.ExecutablePath);

    /// <summary>
    ///     Main method. Needs STAThreadAttribute as this App references System.Windows.Forms
    /// </summary>
    /// <param name="args"></param>
    [STAThreadAttribute]
    private static int Main(string[] args)
    {
        //ApplicationConfiguration.Initialize();
        //Application.Run(new Form1());
        Statics.InitializeLogger();
        // get configuration from command line parameters

        Configuration conf;
        try
        {
            conf = Configuration.create(args);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return 1;
        }

        if (conf.Help)
        {
            ShowHelpMessage();
            return 0;
        }

        // make sure only one instance is online

        if (!conf.AllowMultiple && !InstanceIsUnique())
        {
            Console.Error.WriteLine("Only one instance of process allowed. User -m for muliple instances.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return 1;
        }

        // check if http listener is supported

        if (!HttpListener.IsSupported)
        {
            Console.Error.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return 1;
        }

        try
        {
            // run server
            RunServer(conf);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return 2;
        }

        return 1;
    }

    /// <summary>
    ///     Find out if there are one or more instances of this program running
    /// </summary>
    /// <returns>n processes</returns>
    private static bool InstanceIsUnique()
    {
        if (mutex.WaitOne(TimeSpan.Zero, true))
        {
            mutex.ReleaseMutex();
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Run server
    /// </summary>
    /// <param name="conf">server configuration</param>
    private static void RunServer(Configuration conf)
    {
        using (var server = new HttpServer().Start(conf))
        {
            if (server == null || !server.IsRunning())
            {
                Console.Error.WriteLine("Could not start server... Exiting.");
                return;
            }

            server.WaitForDisconnected();
            //server.Stop();
        }
    }
    
    private static void ShowHelpMessage()
    {
        Console.WriteLine($"Syntax: {ApplicationName} [Port to listen] [Options]");
        Console.WriteLine($"Example: {ApplicationName} 6060 -b");
        Console.WriteLine("Options: -ip :\tBind ip;");
        Console.WriteLine("         -b  :\tDon't show banner message;");
        Console.WriteLine("         -m  :\tAllow multiple instances;");
        Console.WriteLine("         -r  :\tRead only;");
        Console.WriteLine("         -u  :\tLogin username;");
        Console.WriteLine("         -p  :\tLogin password;");
        Console.WriteLine("         -h  :\tHelp (This screen);");
        //Console.WriteLine("\t-i :\tInstall as Windows service");
        //Console.WriteLine("\t-u :\tUninstall as Windows service");
    }
}
