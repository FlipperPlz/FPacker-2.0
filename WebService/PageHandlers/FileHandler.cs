#region imports

using System.Net;
using System.Reflection;
using System.Windows.Forms;
using FPackerLibrary;
#endregion

namespace WebService.PageHandlers;
// This class is responsable for generating all the HTML and javascript the the application homepage

internal class FileHandler : AbstractPageHandler
{
    /// <summary>
    ///     Get the file and safe it.
    /// </summary>
    /// <param name="response">not used</param>
    /// <param name="uri">raw URI tokenized by '/'</param>
    /// <returns>HTML page + javascript</returns>
    public override byte[] HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string[] uri)
    {
        if (request.Headers.GetValues("Content-Length").First() == "0")
        {
            response.StatusCode = 500;
            return new byte[0];
        }
#if DEBUG
        var tempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test");
        var tempFileName = Path.Combine(tempFilePath, $"{PBOUtilities.RandomString(10)}.zip");
#else
        var tempFilePath = Path.Combine(Path.GetTempPath(), "FPacker");
        var tempFileName = Path.Combine(tempFilePath, $"{PBOUtilities.RandomString(10)}.zip");
#endif
        Directory.CreateDirectory(tempFilePath);
        var Boundary = Statics.GetBoundary(request.ContentType);
        if (!Statics.SaveFile(request.ContentEncoding, Boundary, request.InputStream, tempFileName))
        {
            response.StatusCode = 500;
            return new byte[0];
        }
        
        var staticPage = "";
        return BuildHTML(staticPage);
    }
}