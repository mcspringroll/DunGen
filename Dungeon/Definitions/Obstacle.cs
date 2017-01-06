using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Obstacle : PhysicalEntity
    {
        public Obstacle(Room spawnRoom) : base(spawnRoom)
        {
        }
        public bool BlocksVision { get; set; }
        public bool BlocksAllMovement { get; set; }
        public bool BlocksGroundMovement { get; set; }
        public bool Breakable { get; set; }
        public bool Movable { get; set; }
    }
}
