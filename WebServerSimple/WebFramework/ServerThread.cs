using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace WebFramework
{
  public class ServerThread
  {
    private const int INOUT_PAD = 3;

    public static Assembly ExecutingAssembly { get; set; }
    public static string Namespace { get; set; }

    //HTTP-codes see: https://de.wikipedia.org/wiki/HTTP-Statuscode
    private const int HTTP_OK = 200;
    private const int HTTP_BAD_REQUEST = 400;
    private const int HTTP_NOT_FOUND = 404;
    private const int HTTP_INTERNAL_SERVER_ERROR = 500;

    private readonly TcpClient connection;

    public ServerThread(TcpClient connection) => this.connection = connection;

    public void Start() => new Thread(Run).Start();

    #region --------------------------------------------------------- helpers
    private static void PrintWithColor(string header, ConsoleColor colBack, ConsoleColor colFore, string contents = null, int padLeft = 0)
    {
      var bgCol = Console.BackgroundColor;
      var foreCol = Console.ForegroundColor;
      Console.BackgroundColor = colBack;
      Console.ForegroundColor = colFore;
      if (padLeft > 0) header = header.PadLeft(padLeft, ' ');
      Console.Write(header);
      Console.BackgroundColor = bgCol;
      Console.ForegroundColor = foreCol;
      if (contents != null) Console.WriteLine($" {contents}");
    }

    private static List<string> ReadAllLinesFromStream(StreamReader inStream)
    {
      var linesIn = new List<string>();
      while (true)
      {
        string line = inStream.ReadLine();
        if (line == null || line.Trim().Length == 0) break;
        linesIn.Add(line);
      }
      return linesIn;
    }

    private static int Write2Stream(Stream outStream, string s, bool logToConsole = false, ConsoleColor foreColor = ConsoleColor.Cyan)
    {
      try
      {
        var bytesToSend = Encoding.ASCII.GetBytes($"{s}\r\n");
        outStream.Write(bytesToSend, 0, bytesToSend.Length);
        if (!logToConsole) return bytesToSend.Length;
        PrintWithColor("<--", foreColor, ConsoleColor.Black, s, INOUT_PAD);
        return bytesToSend.Length;
      }
      catch (Exception exc)
      {
        Console.WriteLine(exc);
        return 0;
      }
    }

    private static void WriteResponse(Stream outStream, string responseContent, string mime, int code)
    {
      Console.WriteLine("Preparing response");
      if (responseContent == null) responseContent = "<html><body><h1>Die angeforderte Seite existiert nicht</h1></body></html>";
      Write2Stream(outStream, $"HTTP/1.1 {code} OK", true, code == HTTP_OK ? ConsoleColor.Cyan : ConsoleColor.Red);
      Write2Stream(outStream, $"Date: {DateTime.Now:ddd, dd MMM yyyy hh:mm:ss} GMT", true);
      Write2Stream(outStream, "Server: WebServerSimple 1.0", true);
      Write2Stream(outStream, $"Content-Length: {responseContent.Length}", true);
      Write2Stream(outStream, $"Content-Type: {mime}", true);
      Write2Stream(outStream, "");
      int nrBytes = Write2Stream(outStream, responseContent);
      string bodyHead = new string(responseContent.ToString().Replace(Environment.NewLine, "").Take(40).ToArray());
      PrintWithColor("<--", ConsoleColor.Cyan, ConsoleColor.Black, $"{bodyHead}...", INOUT_PAD);
    }

    private static void WriteErrorResponse(Stream outStream, int responseCode, string desc)
    {
      PrintWithColor($"> *** {responseCode}: {desc} ***", ConsoleColor.Red, ConsoleColor.White, "");
      var sb = new StringBuilder();
      sb.Append("<html><head><title>WebServerSimple-Error</title></head<body>");
      sb.Append($"<h1>HTTP/1.1 {responseCode}</h1>");
      sb.Append($"<h3>{desc}</h3>");
      sb.Append("</body></html>");
      WriteResponse(outStream, sb.ToString(), "text/html", responseCode);
    }
    #endregion

    #region --------------------------------------------------------- framework
    public void Run()
    {
      Console.WriteLine();
      Console.WriteLine("____________________________________________ Client connected");
      var outStream = connection.GetStream();
      var inStream = new StreamReader(connection.GetStream());
      HandleRequest(inStream, outStream);
      connection.Close();
      Console.WriteLine("____________________________________________ Client disconnected");
    }

    private static void HandleRequest(StreamReader inStream, Stream outStream)
    {
      try
      {
        string request = ReadRequest(inStream);                   //1 - ReadRequest
        string mime = MimeMapper.GetMimeType(request);            //2 - detect MIME
        string responseContent = PrepareResponse(request);        //3 - PrepareResponse
        int status = responseContent == null ? HTTP_NOT_FOUND : HTTP_OK;
        WriteResponse(outStream, responseContent, mime, status);  //4 - WriteResponse
      }
      catch (Exception exc)
      {
           Console.WriteLine(exc);
           WriteErrorResponse(outStream, HTTP_INTERNAL_SERVER_ERROR, exc.Message);
      }
    }

    private static string ReadRequest(StreamReader inStream)
    {
      var linesIn = ReadAllLinesFromStream(inStream);
      linesIn.ForEach(x => PrintWithColor("-->", ConsoleColor.Green, ConsoleColor.Black, x, INOUT_PAD));
      if (!linesIn.Any()) return null;
      var request = linesIn.First();
      return request;
    }

    public static string PrepareResponse(string request)
    {
            //GET /index.html HTTP/1.1
            var items = request.Split(' ');
            string url = items[1];
            string filename = url.Substring(1, url.Length - 1);
            if (filename.Length == 0) filename = "index.html";

            if (filename.Contains(".")) return PrepareStaticFile(filename);
            return PrepareRoute(filename);

            
    }

        private static string PrepareStaticFile(string filename)
        {
            string path = $@"./wwwroot/{filename}";
            Console.WriteLine(path);
            if (!File.Exists(path)) return null;
            string html = File.ReadAllText(path);
            return html;
        }

        private static string PrepareRoute(string route)
        {
            //Home/Index
            var split = route.Split('/');
            var controller = split[0];
            var action = split[1];
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type type = assembly.GetType($"WebServerSimple.Controllers.{controller}Controller");
            object instance = Activator.CreateInstance(type, null);
            MethodInfo methodInfo = type.GetMethod(action);
            object result = methodInfo.Invoke(instance, null);
            return result as string;
        }

        #endregion

    }
}
