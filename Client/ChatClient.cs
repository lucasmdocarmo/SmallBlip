using Domain;
using Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public class ChatClient
    {
        public static bool firstRequest = true;
        public static void Main()
        {
            using var _tcp = new TcpClient();

            _tcp.Connect(ChatServer.SERVER_IP, ChatServer.PORT_NO);
            NetworkStream _networkStream = _tcp.GetStream();

            Thread thread = new Thread(o => ReceiveData(_networkStream));
            thread.Start(_tcp);

            Console.WriteLine("Welcome to the Chat!!");
            byte[] decisionBuffer = new byte[1024];
            string _initial;
            Console.WriteLine("Whats your name? ");
            while ((_initial = Console.ReadLine()).ToString() != "/exit")
            {
                firstRequest = false;
                byte[] buffer = Encoding.ASCII.GetBytes(_initial);
                _networkStream.Write(buffer, 0, buffer.Length);
            }

            _tcp.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            _networkStream.Close();
            _tcp.Close();

            Console.WriteLine("Desconectado do servidor!!");
            Console.ReadKey();
        }

        private static void ReceiveData(NetworkStream _networkStream)
        {
            byte[] receivedBytes = new byte[1024 * 4];
            int byte_count;

            while ((byte_count = _networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                if (!firstRequest)
                {
                    Console.Write(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
                }
            }
        }
    }
}
