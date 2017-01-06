using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Dungeon
    {
        public Floor CurrentFloor
        {
            get; private set;
        }

        public Player CurrentPlayer
        {
            get; private set;
        }

        public Dungeon()
        {
            CurrentFloor = new Floor();
            CurrentPlayer = new Player(CurrentFloor.CurrentRoom);
        }
    }
}
