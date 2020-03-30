using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Room
    {
        public Room(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
