using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using Domain;
using Service.Repository;

namespace Service
{
    public class Worker
    {
        private readonly IServiceRepository _serviceRepository;
        public Worker()
        {
            _serviceRepository = new ServiceRepository();
        }

        public virtual void WorkerService(List<User> entryUsers, object _object)
        {
            int id = (int)_object;
            TcpClient client;
            var entryUser = entryUsers.FirstOrDefault();
            client = entryUsers.FirstOrDefault(x => x.Id == id).Client;

            while (true)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int byte_count = stream.Read(buffer, 0, buffer.Length);

                    if (byte_count == 0) { break; }

                    string data = Encoding.ASCII.GetString(buffer, 0, byte_count);
                    var selfClient = entryUsers.FirstOrDefault(x => x.Id == id);
                    var otherClients = entryUsers.Where(x => x.Id != id && x.RoomName == selfClient.RoomName);
                    if (string.IsNullOrEmpty(entryUsers.FirstOrDefault(x => x.Id == id).Name))
                    {
                        _serviceRepository.InicialRoom();
                        _serviceRepository.NewUser(data, selfClient, entryUsers.Where(x => x.Id != id && x.RoomName == selfClient.RoomName));

                    }
                    else
                    {
                        var first = data.Split(' ')[0];
                        switch (first)
                        {
                            case "/p":
                                _serviceRepository.PrivateMessage(data, selfClient, otherClients);
                                break;
                            case "/to":
                                _serviceRepository.DirectionMessage(data, selfClient, otherClients);
                                break;
                            case "/users":
                                _serviceRepository.GetConnectedUsers(selfClient.Client, entryUsers);
                                break;
                            case "/help":
                                _serviceRepository.GetCommands(selfClient.Client);
                                break;
                            case "/getrooms":
                                _serviceRepository.GetRooms(selfClient.Client, entryUsers);
                                break;
                            case "/newroom":
                                Console.WriteLine("Creating new Room.....Room Name:");
                                string _chatData = Console.ReadLine();
                                _serviceRepository.NewRoom(_chatData, selfClient);
                                break;
                            case "/changeroom":
                                Console.WriteLine("Changing to new Room.....Room Name:");
                                data = Console.ReadLine();
                                _serviceRepository.ChangeRoom(data, selfClient);
                                _serviceRepository.BroadcastMessages($"{selfClient.Name} Joined In", otherClients.Select(x => x.Client).ToList());
                                break;
                            default:
                                _serviceRepository.BroadcastMessages($" (To Everyone) {selfClient.Name} Says:  {data}", otherClients.Select(x => x.Client).ToList());
                                break;
                        };
                        Console.WriteLine($"{selfClient.Name} say:  {data}");
                    }
                }

                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            entryUsers.Remove(entryUsers.FirstOrDefault(x => x.Id != id));
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
