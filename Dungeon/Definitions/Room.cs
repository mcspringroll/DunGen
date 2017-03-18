using System;
using System.Collections.Generic;
using DungeonAPI.Definitions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Room <TRoom> where TRoom : Room<TRoom>, new()
    {
        #region Properties
        public TRoom North { get; set; }
        public TRoom East { get; set; }
        public TRoom South { get; set; }
        public TRoom West { get; set; }

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
        public List<Enemy<TRoom>> Enemies { get; set; }
        public int X { get; internal set; }
        public int Y { get; internal set; }
        public bool IsClear
        {
            get
            {
                foreach (Enemy<TRoom> e in Enemies)
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

        public void DetachAllNeighbors()
        {
            if (this.North != null)
            {
                this.North = null;
            }
            if (this.East != null)
            {
                this.East = null;
            }
            if (this.South != null)
            {
                this.South = null;
            }
            if (this.West != null)
            {
                this.West = null;
            }

        }
        
        /// <summary>
        /// Creates a new Room object with the same values
        /// as this Room object.  For reference values,
        /// references are preserved.  This could cause
        /// issues if done recklessly.
        /// </summary>
        /// <returns></returns>
        public Room<TRoom> Clone()
        {
            return (Room<TRoom>)MemberwiseClone();
        }

        #region SetConnectionMethods
        public bool SetToNorthOf(TRoom southernRoom)
        {
            if (southernRoom == null || southernRoom.North != null || this.South != null)
                return false;
            southernRoom.North = (TRoom)this;
            this.South = southernRoom;
            return true;
        }

        public bool SetToEastOf(TRoom westernRoom)
        {
            if (westernRoom == null || westernRoom.East != null || this.West != null)
                return false;
            westernRoom.East = (TRoom)this;
            this.West = westernRoom;
            return true;
        }

        public bool SetToSouthOf(TRoom northernRoom)
        {
            if (northernRoom == null || northernRoom.South != null || this.North != null)
                return false;
            northernRoom.South = (TRoom)this;
            this.North = northernRoom;
            return true;
        }

        public bool SetToWestOf(TRoom easternRoom)
        {
            if (easternRoom == null || easternRoom.West != null || this.East != null)
                return false;
            easternRoom.West = (TRoom)this;
            this.East = easternRoom;
            return true;
        }
        #endregion
    }
}
