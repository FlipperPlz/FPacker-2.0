#region imports

using System;

#endregion

namespace WebService;

internal class Configuration
{
    /// <summary>
    ///     Private c'tor with default values for object instances
    ///     Use factory method instead of instatiating c'tor
    /// </summary>
    private Configuration()
    {
        AllowMultiple = false;
        Banner = true;
        Help = false;
        Port = 80;
        IpAddress = "*";
        Username = "";
        Password = "";
    }

    /// <summary>
    ///     Allow multiple instances of process
    /// </summary>
    public bool AllowMultiple { get; private set; }

    /// <summary>
    ///     Display banner
    /// </summary>
    public bool Banner { get; private set; }

    /// <summary>
    ///     Display help
    /// </summary>
    public bool Help { get; private set; }

    /// <summary>
    ///     Port where to listen
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    ///     Gets the ip address.
    /// </summary>
    /// <value>The ip address.</value>
    public string IpAddress { get; private set; }

    /// <summary>
    ///     Username for login
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password for login
    /// </summary>
    public string Password { get; set; }

    public static Configuration create(string[] parameters)
    {
        var conf = new Configuration();

        // use default configuration if no parameters exist

        if (parameters.Length == 0) return conf;

        // cycle throught command line using enumerator on parameters array

        var enumerator = parameters.GetEnumerator();
        enumerator.MoveNext();

        var parameter = (string)enumerator.Current;

        // parse if first parameter is a valid integer and use it as a port number for listener

        int port;
        var hasPortParameter = int.TryParse(parameter, out port);

        if (hasPortParameter)
        {
            conf.Port = port;

            // continue parsing parameters (if they exist)
            if (!enumerator.MoveNext()) return conf;
            parameter = (string)enumerator.Current;
        }


        while (true)
        {
            if (parameter.Equals("-m"))
            {
                conf.AllowMultiple = true;
            }
            else if (parameter.Equals("-b"))
            {
                conf.Banner = false;
            }
            else if (parameter.Equals("-h"))
            {
                conf.Help = true;
            }
            else if (parameter.StartsWith("-ip="))
            {
                conf.IpAddress = parameter.Split('=')[1];
            }
            else if (parameter.StartsWith("-u="))
            {
                conf.Username = parameter.Split('=')[1];
            }
            else if (parameter.StartsWith("-p="))
            {
                conf.Password = parameter.Split('=')[1];
            }
            else
            {
                throw new ArgumentException($"Error: {parameter} is an invalid command line parameter.");
            }

            if (!enumerator.MoveNext()) return conf;
            parameter = (string)enumerator.Current;
        }
    }
}