using System;
using System.Linq;

namespace WebFramework
{
    public class MimeMapper
    {
        public static string GetMimeType(string request)
        {
            string url = request.Split(' ')[1];
            string filename = url.Substring(1, url.Length - 1);
            if (filename.Length == 0) filename = "index.html";
            string extension = filename.Split('.').Last();

            if (extension == "css") return "text/css";
            if (extension == "js") return "text/javascript";
            if (extension == "png") return "image/png";
            if (extension == "jpg") return "image/jpeg";
            return "text/html";

        }
    }
}
