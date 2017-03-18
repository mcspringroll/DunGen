using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Player<TRoom> : FightingEntity<TRoom> where TRoom : Room<TRoom>, new()
    {
        public Player(Room<TRoom> spawnRoom) : base(spawnRoom)
        {
            MaxHealth = 3;
            Health = MaxHealth;
        }
    }
}
