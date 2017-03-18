using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Floor <TRoom> where TRoom : Room<TRoom>, new()
    {
        public const int DEFAULT_ROOMS_FOR_FLOOR = 50;
        
        public Floor()
        {
            CurrentRoom = StartRoom = new TRoom();
            allRooms = new Dictionary<long, TRoom>();
            RoomCount = 0;
            AddRoom(CurrentRoom);
        }

        private Dictionary<long, TRoom> allRooms;

        public TRoom StartRoom { get; private set; }

        public TRoom CurrentRoom { get; set; }
        
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

        public bool GoNorth()
        {
            if (CurrentRoom.North == null)
                return false;
            CurrentRoom = CurrentRoom.North;
            return true;
        }

        public bool GoEast()
        {
            if (CurrentRoom.East == null)
                return false;
            CurrentRoom = CurrentRoom.East;
            return true;
        }

        public bool GoSouth()
        {
            if (CurrentRoom.South == null)
                return false;
            CurrentRoom = CurrentRoom.South;
            return true;
        }

        public bool GoWest()
        {
            if (CurrentRoom.West == null)
                return false;
            CurrentRoom = CurrentRoom.West;
            return true;
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

        public bool AddRoom(TRoom roomToAdd)
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
            Dictionary<long, TRoom> walls = new Dictionary<long, TRoom>();

            foreach(TRoom r in allRooms.Values)
            {
                if (r.IsWall)
                    continue;
                if(r.North == null)
                {
                    TRoom newWallRoom = new TRoom();
                    newWallRoom.SetToNorthOf(r);
                    long key = newWallRoom.GetKeyValue();
                    if(!walls.ContainsKey(key))
                        walls.Add(key, newWallRoom);
                }
                if(r.East == null)
                {
                    TRoom newWallRoom = new TRoom();
                    newWallRoom.SetToEastOf(null);
                    long key = newWallRoom.GetKeyValue();
                    if (!walls.ContainsKey(key))
                        walls.Add(key, newWallRoom);
                }
                if (r.South == null)
                {
                    TRoom newWallRoom = new TRoom();
                    newWallRoom.SetToSouthOf(r);
                    long key = newWallRoom.GetKeyValue();
                    if (!walls.ContainsKey(key))
                        walls.Add(key, newWallRoom);
                }
                if (r.West == null)
                {
                    TRoom newWallRoom = new TRoom();
                    newWallRoom.SetToWestOf(r);
                    long key = newWallRoom.GetKeyValue();
                    if (!walls.ContainsKey(key))
                        walls.Add(key, newWallRoom);
                }
            }

            foreach(TRoom wallRoom in walls.Values)
            {
                wallRoom.IsWall = true;
                this.AddRoom(wallRoom);
            }
        }
        

        public List<TRoom> GetRooms()
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

        public TRoom GetRoomToNorth(TRoom room)
        {
            return this.GetRoomAtCoords(room.X, room.Y + 1);
        }

        public TRoom GetRoomToEast(TRoom room)
        {
            return this.GetRoomAtCoords(room.X + 1, room.Y);
        }

        public TRoom GetRoomToSouth(TRoom room)
        {
            return this.GetRoomAtCoords(room.X, room.Y - 1);
        }

        public TRoom GetRoomToWest(TRoom room)
        {
            return this.GetRoomAtCoords(room.X - 1, room.Y);
        }
        
        public TRoom GetRoomAtCoords(int x, int y)
        {
            long key = (((long)x) << 32) + y;
            if (allRooms.ContainsKey(key))
                return allRooms[key];
            return null;
        }
    }
}
