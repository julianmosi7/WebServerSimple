using System;
using System.IO;

namespace WebServerSimple.Controllers
{
    public class NorthwindController
    {
        public string Products()
        {
            Console.WriteLine("NorthwindController::Products");
            string layoutFile = $@"./Views/Shared/Layout.html";
            Console.WriteLine(layoutFile);

            if (!File.Exists(layoutFile)) return null;
            string layout = File.ReadAllText(layoutFile);
            string contents = File.ReadAllText(@"./Views/Northwind/products.html");
            string html = layout.Replace("@RenderBody()", contents);
            return html;
        }

        public string Employees()
        {
            Console.WriteLine("NorthwindController:Employees");
            string layoutFile = $@"./Views/Shared/Layout.html";
            Console.WriteLine(layoutFile);

            if (!File.Exists(layoutFile)) return null;
            string layout = File.ReadAllText(layoutFile);
            string contents = File.ReadAllText(@"./Views/Northwind/employees.html");
            string html = layout.Replace("@RenderBody()", contents);
            return html;            
        }
    }
}
