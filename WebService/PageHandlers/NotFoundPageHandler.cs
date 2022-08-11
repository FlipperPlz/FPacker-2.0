#region imports

using System.Net;

#endregion

namespace WebService.PageHandlers;

internal class NotFoundPageHandler : AbstractPageHandler
{
    /// <summary>
    ///     404 error page, used when client requests and unexisting resource
    /// </summary>
    /// <param name="response"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    public override byte[] HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string[] uri)
    {
        response.StatusCode = 404;
        return BuildHTML("Page not found!");
    }
}