using Domain;
using Service.DomainServices.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Service.Repository
{
    public class RoomRepository : IRoomRepository
    {
        public static Room Room { get; set; }
        public static User User { get; set; }

        public User ChangeRoom(User _user, string _room)
        {
            User = _user;
            User.RoomName = _room;
            return User;
        }

        public Room InitialRoom(string initialRoom)
        {
            Room = new Room(initialRoom);
            return Room;
        }
        public void AddRoom(string roomName)
        {
            Room.Rooms.Add(new Room(roomName));
        }
        public List<Room> GetRooms()
        {
            return Room.Rooms.ToList();
        }


    }
}