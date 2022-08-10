using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebService
{
    internal class Statics
    {
        internal static bool SaveFile(Encoding enc, string boundary, Stream input, string targetPath)
        {
            using var ms = GetStream(enc, boundary, input);
            if (ms.ToArray().Length == 0) return false;

            using var output = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
            output.Write(ms.ToArray());
            
            return true;

        }
        internal static MemoryStream GetStream(Encoding enc, string boundary, Stream input)
        {
            var boundaryBytes = enc.GetBytes(boundary);
            var boundaryLen = boundaryBytes.Length;                  
           
            var buffer = new byte[1024];
            var len = input.Read(buffer, 0, 1024);
            var startPos = -1;

            // Find start boundary
            while (true)
            {
                if (len == 0)
                {
                    throw new Exception("Start Boundaray Not Found");
                }

                startPos = IndexOf(buffer, len, boundaryBytes);
                if (startPos >= 0)
                {
                    break;
                }
                else
                {
                    Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                    len = input.Read(buffer, boundaryLen, 1024 - boundaryLen);
                }
            }

            // Skip four lines (Boundary, Content-Disposition, Content-Type, and a blank)
            for (var i = 0; i < 4; i++)
            {
                while (true)
                {
                    if (len == 0)
                    {
                        throw new Exception("Preamble not Found.");
                    }

                    startPos = Array.IndexOf(buffer, enc.GetBytes("\n")[0], startPos);
                    if (startPos >= 0)
                    {
                        startPos++;
                        break;
                    }
                    else
                    {
                        len = input.Read(buffer, 0, 1024);
                    }
                }
            }

            Array.Copy(buffer, startPos, buffer, 0, len - startPos);
            len = len - startPos;
            using var output = new MemoryStream();
            while (true)
            {
                var endPos = IndexOf(buffer, len, boundaryBytes);
                if (endPos >= 0)
                {
                    if (endPos > 0) output.Write(buffer, 0, endPos - 2);
                    break;
                }
                else if (len <= boundaryLen)
                {
                    throw new Exception("End Boundaray Not Found");
                }
                else
                {
                    var outputLen = len - boundaryLen;                  
                    output.Write(buffer, 0, outputLen);
                    Array.Copy(buffer, outputLen, buffer, 0, boundaryLen);
                    len = input.Read(buffer, boundaryLen, 1024 - boundaryLen) + boundaryLen;
                }
            }
            
            return output;
        }
        internal static string GetBoundary(string ctype) => "--" + ctype.Split(';')[1].Split('=')[1];

        internal static int IndexOf(byte[] buffer, int len, byte[] boundaryBytes)
        {
            for (var i = 0; i <= len - boundaryBytes.Length; i++)
            {
                var match = true;
                for (var j = 0; j < boundaryBytes.Length && match; j++)
                {
                    match = buffer[i + j] == boundaryBytes[j];
                }
                if (match) return i;                
            }
            return -1;
        }

        internal static KeyHandler keyHandler = new KeyHandler();
    }
}
