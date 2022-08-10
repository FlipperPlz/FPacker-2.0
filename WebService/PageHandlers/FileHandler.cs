#region imports

using System.IO.Compression;
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
        var param = GetParams(uri);
        /*param.TryGetValue("key", out var key);
        if (key == null || !Statics.keyHandler.isKeyValid(key))
        {
            response.StatusCode = 500;
            return BuildHTML("invalid key");
        }*/
        foreach (string header in request.Headers)
        {
            Console.WriteLine($"{header}:{request.Headers[header]}");            
        }

#if DEBUG
        var tempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test");
        var tempFileName = Path.Combine(tempFilePath, $"{PBOUtilities.RandomString(10)}.zip");
        var workingPath = tempFileName.Replace(".zip", "//");
#else
        var tempFilePath = Path.Combine(Path.GetTempPath(), "FPacker");
        var tempFileName = Path.Combine(tempFilePath, $"{PBOUtilities.RandomString(10)}.zip");
#endif
        Directory.CreateDirectory(tempFilePath);
        var Boundary = Statics.GetBoundary(request.ContentType);        
        
        if (!Statics.SaveFile(request.ContentEncoding, Boundary, request.InputStream, tempFileName))
        {
            response.StatusCode = 500;
            return BuildHTML("invalid zip file");
        }

        /*
         * Todo:
         * use streams for in and ouput.
        \*

        /*var ms = Statics.GetStream(request.ContentEncoding, Boundary, request.InputStream);
        if (ms.ToArray().Length == 0)
        {
            response.StatusCode = 500;
            return BuildHTML("invalid zip file");
        }*/

        using (var zip = new ZipArchive(new FileStream(tempFileName, FileMode.Open), ZipArchiveMode.Read))
        {
            zip.ExtractToDirectory(tempFileName.Replace(".zip", "//"));            
        }
        File.Delete(tempFileName);
        var outFile = $"{tempFilePath}.pbo";
        new AddonPacker(tempFileName.Replace(".zip", "//"), outFile);
        using var fs = File.OpenRead(outFile);
        var fileName = Path.GetFileName(outFile);
        response.ContentLength64 = fs.Length;
        response.SendChunked = false;
        response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
        response.AddHeader("Content-disposition", $"attachment; filename={fileName}");
        byte[] buffer = new byte[64 * 1024];
        int read;
        using var bw = new BinaryWriter(response.OutputStream);        
        while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
        {
            bw.Write(buffer, 0, read);
            bw.Flush();
        }
        bw.Close();
        fs.Close();
        File.Delete(outFile);
        Directory.Delete(workingPath, true);

        response.StatusCode = (int)HttpStatusCode.OK;
        response.StatusDescription = "OK";
        var staticPage = "ok";
        return BuildHTML(staticPage);
    }
}