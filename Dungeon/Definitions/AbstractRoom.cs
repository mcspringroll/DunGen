using System;
using System.Collections.Generic;

namespace DungeonAPI.Definitions
{
    public abstract class AbstractRoom<TRoom> where TRoom : AbstractRoom<TRoom>, new()
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

        public int X { get; set; }
        public int Y { get; set; }

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
            set
            {
                IsWall = !value;
            }
        }
        #endregion

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
        public AbstractRoom<TRoom> Clone()
        {
            return (AbstractRoom<TRoom>)MemberwiseClone();
        }

        #region SetConnectionMethods
        public bool SetToNorthOf(AbstractRoom<TRoom> southernRoom)
        {
            if (southernRoom == null || southernRoom.North != null || this.South != null)
                return false;
            southernRoom.North = (TRoom)this;
            this.South = (TRoom)southernRoom;
            return true;
        }

        public bool SetToEastOf(AbstractRoom<TRoom> westernRoom)
        {
            if (westernRoom == null || westernRoom.East != null || this.West != null)
                return false;
            westernRoom.East = (TRoom)this;
            this.West = (TRoom)westernRoom;
            return true;
        }

        public bool SetToSouthOf(AbstractRoom<TRoom> northernRoom)
        {
            if (northernRoom == null || northernRoom.South != null || this.North != null)
                return false;
            northernRoom.South = (TRoom)this;
            this.North = (TRoom)northernRoom;
            return true;
        }

        public bool SetToWestOf(AbstractRoom<TRoom> easternRoom)
        {
            if (easternRoom == null || easternRoom.West != null || this.East != null)
                return false;
            easternRoom.West = (TRoom)this;
            this.East = (TRoom)easternRoom;
            return true;
        }
        #endregion
    }
}
