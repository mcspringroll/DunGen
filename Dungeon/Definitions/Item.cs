using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Definitions
{
    public class Item
    {
        public string Name { get; set; }
        public bool DoesDamage { get; set; }

        private int _healthChangeAmount;
        public int HealthChangeAmount
        {
            get
            {
                return (DoesDamage) ? _healthChangeAmount : (_healthChangeAmount * -1);
            }
            set
            {
                if(value <= 0)
                {
                    value *= -1;
                    DoesDamage = false;
                }
                else
                    DoesDamage = true;
                _healthChangeAmount = value;
            }
        }

        public Item(int damage)
        {
            DoesDamage = true;
        }
    }
}
