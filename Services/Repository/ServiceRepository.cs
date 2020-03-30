using Domain;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using Service.Repository;
using Service.DomainServices.Base;
using Service.DomainServices;

namespace Service.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        public IRoomRepository _roomRepository { get; set; }
        public IUserRepository _userRepository { get; set; }

        public ServiceRepository()
        {
            _roomRepository = new RoomRepository();
            _userRepository = new UserRepository();
        }

        public virtual void InicialRoom()
        {
            var room = _roomRepository.InitialRoom("TakeNetRoom");
            _roomRepository.AddRoom(room.Name);
        }
        public virtual void NewRoom(string data, User client)
        {
            var split = data.Split(" ");
            var roomname = $"#{split[0]}";

            _roomRepository.AddRoom(roomname);

            client.RoomName = roomname;
            Console.WriteLine($"Room {client.RoomName} Created.");
            Console.WriteLine($"Room {client.RoomName} !");
        }
        public virtual void ChangeRoom(string data, User client)
        {
            var salas = _roomRepository.GetRooms();
            var checkSalas = salas.Exists(x => x.Name == data);

            if (!checkSalas)
            {
                _roomRepository.AddRoom(data);
                var _client = _roomRepository.ChangeRoom(client, data);
                Console.WriteLine($"Room {_client.RoomName} not found. User changed to new Created room.");
            }
            else if (checkSalas)
            {
                var _client = _roomRepository.ChangeRoom(client, data);
                Console.WriteLine($"User changed to room {_client.RoomName}");
            }


        }
        public virtual void DirectionMessage(string data, User selfClient, IEnumerable<User> otherClients)
        {
            var split = data.Split(" ");
            string message = string.Empty;
            for (int i = 2; i < split.Length; i++)
            {
                message += split[i] + " ";
            }

            BroadcastMessages($"{selfClient.Name} says to: {split[1]}:  {message}", otherClients.Select(x => x.Client).ToList());
        }
        public virtual void NewUser(string data, User _user, IEnumerable<User> otherClients)
        {
            _user.Name = data;
            _user.RoomName = _roomRepository.GetRooms().Where(x => x.Name == "TakeNetRoom").Select(x => x.Name).FirstOrDefault();
            Console.WriteLine();
            GetCommands(_user.Client);

            BroadcastMessages($"{_user.Name} joined the chat", otherClients.Select(x => x.Client).ToList());
        }
        public virtual void PrivateMessage(string data, User selfClient, IEnumerable<User> otherClients)
        {
            var split = data.Split(" ");
            string message = string.Empty;
            for (int i = 2; i < split.Length; i++)
            {
                message += split[i] + " ";
            }

            SingleMessage($"{selfClient.Name} Says: (private message) To: {split[1]}:  {message}", otherClients.FirstOrDefault(x => x.Name == split[1]).Client);
        }
        public virtual void SingleMessage(string data, TcpClient client)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            NetworkStream stream = client.GetStream();
            stream.Write(buffer, 0, buffer.Length);

        }
        public virtual void GetConnectedUsers(TcpClient tcpClient, List<User> users)
        {

            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = Encoding.ASCII.GetBytes("Connected Users: " + Environment.NewLine);
            stream.Write(buffer, 0, buffer.Length);
            foreach (var item in users)
            {
                buffer = Encoding.ASCII.GetBytes(item.Name + Environment.NewLine);
                stream.Write(buffer, 0, buffer.Length);
            }

        }
        public virtual void GetCommands(TcpClient tcpClient)
        {

            Console.WriteLine();
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = Encoding.ASCII.GetBytes($"{Environment.NewLine} Command List: {Environment.NewLine}");
            stream.Write(buffer, 0, buffer.Length);
            foreach (var item in ServerCommands.GetCommands())
            {
                buffer = Encoding.ASCII.GetBytes(item.Action + " - " + item.Description + Environment.NewLine);
                stream.Write(buffer, 0, buffer.Length);
            }

        }
        public virtual void GetRooms(TcpClient tcpClient, List<User> users)
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = Encoding.ASCII.GetBytes("Salas disponíveis: " + Environment.NewLine);
            stream.Write(buffer, 0, buffer.Length);

            foreach (var item in _roomRepository.GetRooms())
            {
                buffer = Encoding.ASCII.GetBytes($" {item.Name} -  {users.Count(x => x.RoomName == item.Name)} usuarios conectados  {Environment.NewLine}");
                stream.Write(buffer, 0, buffer.Length);
            }

        }
        public virtual void BroadcastMessages(string data, List<TcpClient> list_clients)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            foreach (TcpClient c in list_clients)
            {
                NetworkStream stream = c.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
    public class ServerCommands
    {
        public ServerCommands(string action, string description)
        {
            Action = action;
            Description = description;
        }

        public string Action { get; set; }
        public string Description { get; set; }

        public static List<ServerCommands> GetCommands()
        {
            List<ServerCommands> commands = new List<ServerCommands>();
            commands.Add(new ServerCommands("", "---------------------------------------------------------"));
            commands.Add(new ServerCommands("/users", "Get List of Connected Users"));
            commands.Add(new ServerCommands("/p ", "Send A Message To a Private user. (Use spaces after /p username message)"));
            commands.Add(new ServerCommands("/to ", "Send Direct Message To a User.(Use spaces after /to username message"));
            commands.Add(new ServerCommands("/help", "Command List"));
            commands.Add(new ServerCommands("/getrooms", "Room List"));
            commands.Add(new ServerCommands("/newroom", "Create New Server Room"));
            commands.Add(new ServerCommands("/changeroom", "Change To new Room "));
            commands.Add(new ServerCommands("/exit", "Leave Chat"));
            commands.Add(new ServerCommands("-", "Type Anything to Deliver a Public Message"));
            commands.Add(new ServerCommands("", "---------------------------------------------------------"));
            return commands;
        }

        public static bool CheckCommand(string data)
        {
            var commands = GetCommands();
            return commands.Any(x => x.Action.Contains(data.Split(" ")[0]));
        }
    }
}