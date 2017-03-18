using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Dungeon <TRoom> where TRoom : Room<TRoom>, new()
    {
        public Floor<TRoom> CurrentFloor
        {
            get; private set;
        }

        public Player<TRoom> CurrentPlayer
        {
            get; private set;
        }

        public Dungeon()
        {
            CurrentFloor = new Floor<TRoom>();
            CurrentPlayer = new Player<TRoom>(CurrentFloor.CurrentRoom);
        }
    }
}
