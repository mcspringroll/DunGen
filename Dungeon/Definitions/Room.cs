using System;
using System.Collections.Generic;
using DungeonAPI.Definitions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Room
    {
        #region Properties
        public Room North { get; set; }
        public Room East { get; set; }
        public Room South { get; set; }
        public Room West { get; set; }

        public bool IsConnectedToNorth { get; set; }
        public bool IsConnectedToEast { get; set; }
        public bool IsConnectedToSouth { get; set; }
        public bool IsConnectedToWest { get; set; }

        public bool HasAllNeighbors
        {
            get
            {
                return North != null && East != null && South != null && West != null;
            }
        }

        public List<Item> Items { get; set; }
        public List<Enemy> Enemies { get; set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool IsClear
        {
            get
            {
                foreach (Enemy e in Enemies)
                {
                    if (e.IsNotDead)
                        return false;
                }
                return true;
            }
        }
        public bool IsNotClear
        {
            get
            {
                return !IsClear;
            }
        }

        public bool IsWall { get; set; }
        public bool IsNotWall
        {
            get
            {
                return !IsWall;
            }
        }
        #endregion

        public Room() : this(0,0)
        { }

        public Room(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public long GetKeyValue()
        {
            return (((long)X) << 32) + Y;
        }

        #region SetConnectionMethods
        public bool SetToNorthOf(Room southernRoom)
        {
            if (southernRoom == null || southernRoom.North != null || this.South != null)
                return false;
            southernRoom.North = this;
            this.South = southernRoom;
            return true;
        }

        public bool SetToEastOf(Room westernRoom)
        {
            if (westernRoom == null || westernRoom.East != null || this.West != null)
                return false;
            westernRoom.East = this;
            this.West = westernRoom;
            return true;
        }

        public bool SetToSouthOf(Room northernRoom)
        {
            if (northernRoom == null || northernRoom.South != null || this.North != null)
                return false;
            northernRoom.South = this;
            this.North = northernRoom;
            return true;
        }

        public bool SetToWestOf(Room easternRoom)
        {
            if (easternRoom == null || easternRoom.West != null || this.East != null)
                return false;
            easternRoom.West = this;
            this.East = easternRoom;
            return true;
        }
        #endregion
    }
}
