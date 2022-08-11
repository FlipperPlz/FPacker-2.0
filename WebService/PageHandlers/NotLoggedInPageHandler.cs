using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WebService.PageHandlers;
internal class NotLoggedInPageHandler : AbstractPageHandler
{
    /// <summary>
    ///     404 error page, used when client requests and unexisting resource
    /// </summary>
    /// <param name="response"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    public override byte[] HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string[] uri)
    {
        response.StatusCode = 401;
        return BuildHTML("Access denied!");
    }
}
