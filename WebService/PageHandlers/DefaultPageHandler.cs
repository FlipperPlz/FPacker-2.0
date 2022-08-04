#region imports

using System.Net;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace WebService.PageHandlers;
// This class is responsable for generating all the HTML and javascript the the application homepage

internal class DefaultPageHandler : AbstractPageHandler
{
    string fileName;
    internal DefaultPageHandler(string fileName)
    {
        this.fileName = fileName;
    }

    /// <summary>
    ///     Generate the HTML and javascript needed for this the homepage
    /// </summary>
    /// <param name="response">not used</param>
    /// <param name="uri">raw URI tokenized by '/'</param>
    /// <returns>HTML page + javascript</returns>
    public override byte[] HandleRequest(HttpListenerResponse response, string[] uri)
    {
        var staticPage = File.ReadAllText($"{fileName}");
        return BuildHTML(staticPage);
    }
}