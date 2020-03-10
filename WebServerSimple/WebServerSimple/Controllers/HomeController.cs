using System;
using System.IO;

namespace WebServerSimple.Controllers
{
    public class HomeController : WebFramework.ControllerBase
    {
        
        public string Index()
        {
            Console.WriteLine("-----");
            Console.WriteLine(Root);
            Console.WriteLine("-----");

            var model = new
            {
                Hook1 = "This is the contents for hook1",
                Hook2 = "Found hook2 here"
            };

            return View(nameof(Index), "Home", model);
        }

        public string Dummy()
        {
            Console.WriteLine("-----");
            Console.WriteLine(Root);
            Console.WriteLine("-----");
            return View("Dummy", "Home");
        }
    }
}
