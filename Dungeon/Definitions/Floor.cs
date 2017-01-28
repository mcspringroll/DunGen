using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Floor
    {
        public const int DEFAULT_ROOMS_FOR_FLOOR = 50;

        public Floor() : this(DEFAULT_ROOMS_FOR_FLOOR)
        {}

        public Floor(int numberOfRoomsToBuild)
        {
            CurrentRoom = StartRoom = new Room();
            allRooms = new HashSet<Room>();
            allRooms.Add(CurrentRoom);
            RoomCount = numberOfRoomsToBuild;
        }

        private HashSet<Room> allRooms;

        public Room StartRoom { get; private set; }

        public Room CurrentRoom { get; set; }


        public int RoomCount { get; private set; }

        public int Width
        {
            get
            {
                int smallestX = 0,
                        largestX = 0;

                foreach (Room r in allRooms)
                {
                    if (r.X < smallestX)
                        smallestX = r.X;
                    if (r.X > largestX)
                        largestX = r.X;
                }

                int distance = largestX - smallestX;

                return distance + 1;
            }
        }

        public int Height
        {
            get
            {
                int smallestY = 0,
                        largestY = 0;

                foreach (Room r in allRooms)
                {
                    if (r.Y < smallestY)
                        smallestY = r.Y;
                    if (r.Y > largestY)
                        largestY = r.Y;
                }

                int distance = largestY - smallestY;

                return distance + 1;
            }
        }

        public int SmallestX
        {
            get
            {
                int smallestX = 0;

                foreach (Room r in allRooms)
                {
                    if (r.X < smallestX)
                    {
                        smallestX = r.X;
                    }
                }

                return smallestX;
            }
        }

        public int SmallestY
        {
            get
            {
                int smallestY = 0;

                foreach (Room r in allRooms)
                {
                    if (r.Y < smallestY)
                    {
                        smallestY = r.Y;
                    }
                }

                return smallestY;
            }
        }

        public int LargestX
        {
            get
            {
                int largestX = 0;

                foreach (Room r in allRooms)
                {
                    if (r.X > largestX)
                    {
                        largestX = r.X;
                    }
                }

                return largestX;
            }
        }

        public int LargestY
        {
            get
            {
                int largestY = 0;

                foreach (Room r in allRooms)
                {
                    if (r.Y > largestY)
                    {
                        largestY = r.Y;
                    }
                }

                return largestY;
            }
        }

        public bool AddRoom(Room roomToAdd)
        {
            if (this.HasRoomAtCoords(roomToAdd.X, roomToAdd.Y))
            {
                return false;
            }

            bool success = allRooms.Add(roomToAdd);

            if(success)
            {
                roomToAdd.setToNorthOf(this.GetRoomAtCoords(roomToAdd.X, roomToAdd.Y - 1));
                roomToAdd.setToEastOf(this.GetRoomAtCoords(roomToAdd.X -1, roomToAdd.Y));
                roomToAdd.setToSouthOf(this.GetRoomAtCoords(roomToAdd.X, roomToAdd.Y + 1));
                roomToAdd.setToWestOf(this.GetRoomAtCoords(roomToAdd.X + 1, roomToAdd.Y));
            }

            return success;
        }

        public HashSet<Room> GetRooms()
        {
            return allRooms;
        }

        public bool DoesNotHaveRoomAtCoords(int x, int y)
        {
            return !HasRoomAtCoords(x, y);
        }

        public bool HasRoomAtCoords(int x, int y)
        {
            return GetRoomAtCoords(x, y) != null;
        }

        public Room GetRoomToNorth(Room room)
        {
            return this.GetRoomAtCoords(room.X, room.Y + 1);
        }

        public Room GetRoomToEast(Room room)
        {
            return this.GetRoomAtCoords(room.X + 1, room.Y);
        }

        public Room GetRoomToSouth(Room room)
        {
            return this.GetRoomAtCoords(room.X, room.Y - 1);
        }

        public Room GetRoomToWest(Room room)
        {
            return this.GetRoomAtCoords(room.X - 1, room.Y);
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
        public Room GetRoomAtCoords(int x, int y)
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
