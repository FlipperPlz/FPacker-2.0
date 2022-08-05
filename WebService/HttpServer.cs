using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using WebService.PageHandlers;

namespace WebService;

internal sealed class HttpServer : IDisposable
{
    public HttpServer()
    {
        // Homepage 
        decoder.Add("", new HomePageHandler());
        //decoder.Add("home", new HomePageHandler());
        foreach (var page in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HTML\\"), "*.html"))
        {
            Console.WriteLine($"HTML\\{page.Split("\\").Last()} at {page.Split("\\").Last().Replace(".html", "")}");
            decoder.Add(page.Split("\\").Last().Replace(".html", ""), new DefaultPageHandler(page));
        }
        // image of choosen device as a png
        decoder.Add("files", new FileHandler());
        
        // image of choosen device as a png
        decoder.Add("favicon.ico", new IconPageHandler());

        // 404 error page
        decoder.Add("404", new NotFoundPageHandler());

        // 401 error page
        decoder.Add("401", new NotLoggedInPageHandler());
    }

    /// <summary>
    ///     Keep list of all resources to be invoked according to received HTTP requests
    /// </summary>
    private readonly Dictionary<string, AbstractPageHandler>
        decoder = new Dictionary<string, AbstractPageHandler>();

    /// <summary>
    ///     HTTP listener for HttpServer
    /// </summary>
    private readonly HttpListener listener = new HttpListener();

    /// <summary>
    ///     Constructor
    /// </summary>
    private Thread thread;
    public void Dispose()
    {
        if (listener != null) listener.Close();

        // some page handlers might need to be disposed

        foreach (var page in decoder.Values)
        {
            if (page is IDisposable)
            {
                ((IDisposable)page).Dispose();
            }
        }
    }

    /// <summary>
    ///     Username for login
    /// </summary>
    private string Username { get; set; }

    /// <summary>
    /// Password for login
    /// </summary>
    private string Password { get; set; }

    /// <summary>
    ///     Start HttpServer
    /// </summary>
    /// <param name="configuration">Configuration to use</param>
    public HttpServer Start(Configuration conf)
    {
        try
        {
            Username = conf.Username;
            Password = conf.Password;
            Console.Error.WriteLine($"Starting HttpServer at {conf.IpAddress}:{conf.Port} for {Username}");
            listener.Prefixes.Add($"http://{conf.IpAddress}:{conf.Port}/");
            listener.IgnoreWriteExceptions = true;
            if (Username != "" && Password != "") listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            //listener.AuthenticationSchemes = AuthenticationSchemes.None;
            listener.Start();

            thread = new Thread(Start);
            thread.Start();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Could not listen on port {conf.Port}.");
            Console.Error.WriteLine(e);
        }

        return this;
    }

    public void WaitForDisconnected()
    {
        thread.Join();
    }

    /// <summary>
    ///     This where the HttpServer runs
    /// </summary>
    private void Start()
    {
        do
        {
            HttpListenerResponse response = null;
            Stream output = null;
            try
            {
                // Note: The GetContext method blocks while waiting for a request. 
                var context = listener.GetContext();
                var loggedIn = true;
                if (Username != "" && Password != "")
                {
                    var identity = (HttpListenerBasicIdentity)context.User.Identity;
                    loggedIn = identity.Name == Username && identity.Password == Password;
                }
#if DEBUG
                Console.Error.WriteLine(context.Request.RawUrl);
                Console.WriteLine(listener.AuthenticationSchemes);
#endif
                AbstractPageHandler page;
                var uri = context.Request.RawUrl.Split('/');
                
                var found = decoder.TryGetValue(uri[1].Split('?').First(), out page);
                if (!loggedIn)
                {
                    // if not logged in display 401 page
                    page = decoder["401"];
                }
                if (!found)
                {
                    // if page not found display 404 page
                    page = decoder["404"];
                }

                
                response = context.Response;

                var buffer = page.HandleRequest(context.Request, response, uri);

                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                if (listener.IsListening) Console.Error.WriteLine(e);
            }
            finally
            {
                if (response != null)
                {
                    try
                    {
                        if (output != null) output.Close();
                    }
                    catch
                    {
                    }

                    response.Close();
                }
            }

            Thread.Sleep(10);
        } while (listener.IsListening);
    }

    /// <summary>
    ///     Stop HttpServer
    /// </summary>
    public void Stop()
    {
        listener.Stop();
    }

    /// <summary>
    ///     is HttpServer running?
    /// </summary>
    /// <returns></returns>
    public bool IsRunning() => listener.IsListening;
}



