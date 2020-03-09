using System.Reflection;
using WebFramework;

namespace WebServerSimple
{
  public class Program
  {
    public static void Main()
    {
      ServerThread.ExecutingAssembly = Assembly.GetExecutingAssembly();
      ServerThread.Namespace = new Program().GetType().Namespace;
      new WebListener().StartListen(12345);
    }

  }
}
