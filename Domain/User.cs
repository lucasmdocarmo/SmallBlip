using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string To { get; set; }
        public string RoomName { get; set; }
        public TcpClient Client { get; set; }
    }

}
