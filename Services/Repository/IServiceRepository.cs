using Domain;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using Service.Repository;
namespace Service.Repository
{
    public interface IServiceRepository
    {
        void InicialRoom();
        void NewRoom(string data, User client);
        void ChangeRoom(string data, User client);
        void DirectionMessage(string data, User selfClient, IEnumerable<User> otherClients);
        void NewUser(string data, User _user, IEnumerable<User> otherClients);
        void PrivateMessage(string data, User selfClient, IEnumerable<User> otherClients);
        void SingleMessage(string data, TcpClient client);
        void GetConnectedUsers(TcpClient tcpClient, List<User> users);
        void GetCommands(TcpClient tcpClient);
        void GetRooms(TcpClient tcpClient, List<User> users);
        void BroadcastMessages(string data, List<TcpClient> list_clients);
    }
}
