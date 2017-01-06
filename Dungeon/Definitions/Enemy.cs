using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Enemy : FightingEntity
    {
        public Enemy(Room spawnRoom) : base(spawnRoom)
        {

        }
    }
}
