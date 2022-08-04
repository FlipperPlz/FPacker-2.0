using NLog.Config;

namespace FPackerLibrary
{
    public static class Statics
    {
        public static void InitializeLogger()
        {
            NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"..\..\..\NLog.config", true);
        }
    }
}
