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
        
        public Floor()
        {
            CurrentRoom = StartRoom = new Room();
            allRooms = new Dictionary<long, Room>();
            RoomCount = 0;
            AddRoom(CurrentRoom);
        }

        private Dictionary<long, Room> allRooms;

        public Room StartRoom { get; private set; }

        public Room CurrentRoom { get; set; }


        public int RoomCount { get; private set; }

        public int Width
        {
            get
            {
                int distance = LargestX - SmallestX;

                return distance + 1;
            }
        }

        public int Height
        {
            get
            {
                int distance = LargestY - SmallestY;

                return distance + 1;
            }
        }

        public int SmallestX { get; private set; }

        public int SmallestY { get; private set; }

        public int LargestX { get; private set; }

        public int LargestY { get; private set; }

        private void UpdateExtremes(int newX, int newY)
        {
            if (newX < SmallestX)
                SmallestX = newX;
            if (newX > LargestX)
                LargestX = newX;
            if (newY < SmallestY)
                SmallestY = newY;
            if (newY > LargestY)
                LargestY = newY;
        }

        public bool AddRoom(Room roomToAdd)
        {
            if (this.HasRoomAtCoords(roomToAdd.X, roomToAdd.Y))
            {
                return false;
            }

            allRooms.Add(roomToAdd.GetKeyValue(), roomToAdd);
            UpdateExtremes(roomToAdd.X, roomToAdd.Y);

            roomToAdd.SetToNorthOf(this.GetRoomAtCoords(roomToAdd.X, roomToAdd.Y - 1));
            roomToAdd.SetToEastOf(this.GetRoomAtCoords(roomToAdd.X - 1, roomToAdd.Y));
            roomToAdd.SetToSouthOf(this.GetRoomAtCoords(roomToAdd.X, roomToAdd.Y + 1));
            roomToAdd.SetToWestOf(this.GetRoomAtCoords(roomToAdd.X + 1, roomToAdd.Y));

            RoomCount ++;

            return true;
        }

        /// <summary>
        /// Places wall rooms adjacent to every room
        /// in the floor.
        /// </summary>
        public void WallAllRooms()
        {
            Dictionary<long, Room> walls = new Dictionary<long, Room>();

            foreach(Room r in allRooms.Values)
            {
                if (r.IsWall)
                    continue;
                if(r.North == null)
                {
                    Room newWallRoom = new Room(r.X, r.Y + 1);
                    long key = newWallRoom.GetKeyValue();
                    if(!walls.ContainsKey(key))
                        walls.Add(key, newWallRoom);
                }
                if(r.East == null)
                {
                    Room newWallRoom = new Room(r.X + 1, r.Y);
                    long key = newWallRoom.GetKeyValue();
                    if (!walls.ContainsKey(key))
                        walls.Add(key, newWallRoom);
                }
                if (r.South == null)
                {
                    Room newWallRoom = new Room(r.X, r.Y - 1);
                    long key = newWallRoom.GetKeyValue();
                    if (!walls.ContainsKey(key))
                        walls.Add(key, newWallRoom);
                }
                if (r.West == null)
                {
                    Room newWallRoom = new Room(r.X - 1, r.Y);
                    long key = newWallRoom.GetKeyValue();
                    if (!walls.ContainsKey(key))
                        walls.Add(key, newWallRoom);
                }
            }

            foreach(Room wallRoom in walls.Values)
            {
                wallRoom.IsWall = true;
                this.AddRoom(wallRoom);
            }
        }
        

        public List<Room> GetRooms()
        {
            return allRooms.Values.ToList();
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
        
        public Room GetRoomAtCoords(int x, int y)
        {
            long key = (((long)x) << 32) + y;
            if (allRooms.ContainsKey(key))
                return allRooms[key];
            return null;
        }
    }
}
