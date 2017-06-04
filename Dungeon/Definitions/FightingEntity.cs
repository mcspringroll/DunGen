namespace DungeonAPI.Definitions
{
    public abstract class FightingEntity<TRoom> : DamagableEntity<TRoom> where TRoom : AbstractRoom<TRoom>, new()
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

        public FightingEntity(AbstractRoom<TRoom> spawnRoom) : base(spawnRoom)
        {

        }
    }
}
