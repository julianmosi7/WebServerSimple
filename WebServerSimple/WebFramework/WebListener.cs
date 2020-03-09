using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebFramework
{
  public class WebListener
  {
    public void StartListen(int port)
    {
      var listener = new TcpListener(IPAddress.Any, port);
      listener.Start();
      Console.WriteLine($@"Usage: http://localhost:{port} or http://localhost:{port}/Home/Index");
      Console.WriteLine($"Listening on port {port} ...");
      while (true)
      {
        var tcpClient = listener.AcceptTcpClient();
        new ServerThread(tcpClient).Start();
        Thread.Sleep(100); //ServerThread is not thread safe
      }
      //listener.Stop();
    }
  }
}
