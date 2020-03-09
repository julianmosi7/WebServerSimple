using System;
using System.IO;

namespace WebServerSimple.Controllers
{
    public class HomeController
    {
        public string Index()
        {
            Console.WriteLine("HomeController:Index");
            string layoutFile = $@"./Views/Home/index.html";
            Console.WriteLine(layoutFile);

            if (!File.Exists(layoutFile)) return null;
            string layout = File.ReadAllText(layoutFile);
            string contents = File.ReadAllText($@"./Views/Home/index.html");
            string html = layout.Replace("@RenderBody()", contents);
            return html;
        }

        public string Dummy()
        {
            Console.WriteLine("HomeController:Dummy");
            string layoutFile = $@"./Views/Home/Layout.html";
            Console.WriteLine(layoutFile);

            if (!File.Exists(layoutFile)) return null;
            string layout = File.ReadAllText(layoutFile);
            string contents = "<ul>";
            for (int i = 0; i < 5; i++)
            {
                contents += $"<li>{i}</li>";

            }
            contents += "</ul>";
            string html = layout.Replace("@RenderBody()", contents);
            return html;
        }
    }
}
