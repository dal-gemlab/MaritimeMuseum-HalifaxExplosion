using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;


namespace WebSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var wssv = new WebSocketSharp.Server.WebSocketServer("ws://localhost:8888");
            wssv.AddWebSocketService<StreamingMessages>("/ws");
            wssv.Start();
            Console.WriteLine("Webserver running. Press any key to close");
            Console.ReadKey(true);
            wssv.Stop();
        }
    }

    public class StreamingMessages : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Send(e.Data);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Console.WriteLine("Client Connected");
        }
    }
}
