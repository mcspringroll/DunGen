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

        public Room()
        {
            this.X = 0;
            this.Y = 0;
        }

        #region SetConnectionMethods
        public bool setToNorthOf(Room southernRoom)
        {
            if (southernRoom == null || southernRoom.North != null || this.South != null)
                return false;
            southernRoom.North = this;
            this.South = southernRoom;
            this.X = southernRoom.X;
            this.Y = southernRoom.Y + 1;
            return true;
        }

        public bool setToEastOf(Room westernRoom)
        {
            if (westernRoom == null || westernRoom.East != null || this.West != null)
                return false;
            westernRoom.East = this;
            this.West = westernRoom;
            this.X = westernRoom.X + 1;
            this.Y = westernRoom.Y;
            return true;
        }

        public bool setToSouthOf(Room northernRoom)
        {
            if (northernRoom == null || northernRoom.South != null || this.North != null)
                return false;
            northernRoom.South = this;
            this.North = northernRoom;
            this.X = northernRoom.X;
            this.Y = northernRoom.Y - 1;
            return true;
        }

        public bool setToWestOf(Room easternRoom)
        {
            if (easternRoom == null || easternRoom.West != null || this.East != null)
                return false;
            easternRoom.West = this;
            this.East = easternRoom;
            this.X = easternRoom.X - 1;
            this.Y = easternRoom.Y;
            return true;
        }
        #endregion
    }
}
