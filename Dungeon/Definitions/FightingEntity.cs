using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public abstract class FightingEntity : DamagableEntity
    {
        public Item EquippedItem { get; set; }

        public string EquippedItemName
        {
            get
            {
                if (EquippedItem == null)
                    return "Nothing Equipped";
                return EquippedItem.Name;
            }
        }

        private int _damage;
        public int Damage
        {
            get
            {
                if(EquippedItem.DoesDamage)
                {
                    return _damage + EquippedItem.HealthChangeAmount;
                }
                return EquippedItem.HealthChangeAmount;
            }
            set
            {
                _damage = (value < 0) ? 0 : value;
            }
        }
        public void hurt(int damage)
        {
            Health -= damage;
        }

        public FightingEntity(Room spawnRoom) : base(spawnRoom)
        {

        }
    }
}
