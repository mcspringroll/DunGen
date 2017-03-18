using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Enemy<TRoom> : FightingEntity<TRoom> where TRoom : Room<TRoom>, new()
    {
        public Enemy(TRoom spawnRoom) : base(spawnRoom)
        {

        }
    }
}
