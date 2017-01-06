using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Player : FightingEntity
    {
        public Player(Room spawnRoom) : base(spawnRoom)
        {
            MaxHealth = 3;
            Health = MaxHealth;
        }
    }
}
