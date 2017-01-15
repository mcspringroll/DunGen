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

        public bool doesNotHaveRoomAtCoords(int x, int y)
        {
            return !hasRoomAtCoords(x, y);
        }

        public bool hasRoomAtCoords(int x, int y)
        {
            return getRoomAtCoords(x, y) != null;
        }

        public Room getRoomToNorth(Room room)
        {
            return room.North;
        }

        public Room getRoomToEast(Room room)
        {
            return room.East;
        }

        public Room getRoomToSouth(Room room)
        {
            return room.South;
        }

        public Room getRoomToWest(Room room)
        {
            return room.West;
        }

        /* this seems inefficient somehow? can we store this as an array?
         * since we have coordinates anyway? 
         * is there some sort of array list structure in c#?
         * could we add each room to the proper row or column and then
         * sort that array of rooms, to keep a properly ordered set of rows or
         * columns, and then lookup would be much faster?
         * would need to pad with null rooms to keep a rectangle, though
         * is this worth it, in terms of memory and time spent sorting?
         * where is the hasRoomAtCoords method used, anyway?
         * your comment in executeNextCommand 7b is something about
         * if there is no room to the north, suggesting we could just use
         * .North from the current room instead of hasRoomAtCoords
         */
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
