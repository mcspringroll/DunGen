using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public abstract class PhysicalEntity
    {
        public int XLocation { get; set; }
        public int YLocation { get; set; }

        public Room CurrentRoom { get; set; }
        
        public void move(int moveX, int moveY)
        {
            XLocation += moveX;
            YLocation += moveY;
        }
        
        public PhysicalEntity(Room spawnRoom)
        {
            CurrentRoom = spawnRoom;
            XLocation = 0;
            YLocation = 0;
        }
    }
}
