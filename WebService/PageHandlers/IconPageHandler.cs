#region imports

using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

#endregion

namespace WebService.PageHandlers;

internal class IconPageHandler : AbstractPageHandler
{
    /// <summary>
    ///     c'tor, creates chached copy of favicon
    /// </summary>
    public IconPageHandler()
    {
        using (var icon = new MemoryStream())
        {
            var bitmap = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            bitmap.Save(icon);
            buffer = icon.GetBuffer();
        }
    }

    /// <summary>
    ///     cached copy of favicon
    /// </summary>
    private readonly byte[] buffer;

    /// <summary>
    ///     not much to do here, just return the cached favicon
    /// </summary>
    /// <param name="response"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    public override byte[] HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string[] uri) => buffer;
}