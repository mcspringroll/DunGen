using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Floor
    {
        private HashSet<Room> allRooms;
        public Room StartRoom { get; private set; }
        public Room CurrentRoom { get; set; }

        public int RoomCount { get; private set; }

        public Floor()
        {
            CurrentRoom = StartRoom = new Room();
            allRooms = new HashSet<Room>();
            allRooms.Add(CurrentRoom);
            //build dungeon here, lol
        }

        public bool hasRoomAtCoords(int x, int y)
        {
            return getRoomAtCoords(x, y) != null;
        }

        public Room getRoomToNorth(Room room)
        {
            return getRoomAtCoords(room.X, room.Y + 1);
        }

        public Room getRoomToEast(Room room)
        {
            return getRoomAtCoords(room.X + 1, room.Y);
        }

        public Room getRoomToSouth(Room room)
        {
            return getRoomAtCoords(room.X, room.Y - 1);
        }

        public Room getRoomToWest(Room room)
        {
            return getRoomAtCoords(room.X - 1, room.Y);
        }

        public Room getRoomAtCoords(int x, int y)
        {
            foreach (Room r in allRooms)
            {
                if (r.X == x && r.Y == y)
                    return r;
            }
            return null;
        }
    }
}
