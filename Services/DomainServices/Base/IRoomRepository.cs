using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.DomainServices.Base
{
    public interface IRoomRepository
    {
        User ChangeRoom(User user, string _room);
        Room InitialRoom(string _room);
        List<Room> GetRooms();
        void AddRoom(string roomName);
    }
}
