using Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Service;

namespace Server
{
    public class ChatServer 
    {
        public static int PORT_NO = 5000;
        public static string SERVER_IP = "127.0.0.1";
        public static TcpClient tcpClient = new TcpClient();
        public static List<User> entryUsers { get; set; } = new List<User>();

        public static void Main()
        {
            int count = 1;
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5000);
            ServerSocket.Start();
            while (true)
            {
                TcpClient tcpClient = ServerSocket.AcceptTcpClient();
                Console.WriteLine("Server Console Started");
                Thread _thread = new Thread(CallService);

                _thread.Start(count);
                entryUsers.Add(new User() { Id = count, Client = tcpClient });

                count++;
            }
        }
        public static void CallService(object _objectThread)
        {
            Worker _worker = new Worker();
            _worker.WorkerService(entryUsers, _objectThread);
        }


    }
}
