#region imports

using System.Net;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace WebService.PageHandlers;
// This class is responsable for generating all the HTML and javascript the the application homepage

internal class HomePageHandler : AbstractPageHandler
{
    /// <summary>
    ///     Generate the HTML and javascript needed for this the homepage
    /// </summary>
    /// <param name="response">not used</param>
    /// <param name="uri">raw URI tokenized by '/'</param>
    /// <returns>HTML page + javascript</returns>
    public override byte[] HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string[] uri)
    {
        var staticPage = File.ReadAllText($"HTML/{MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("Handler", "")}.html");
        return BuildHTML(staticPage);
    }
}