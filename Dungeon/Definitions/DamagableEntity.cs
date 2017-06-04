namespace DungeonAPI.Definitions
{
    public abstract class DamagableEntity<TRoom> : PhysicalEntity<TRoom> where TRoom : AbstractRoom<TRoom>, new()
    {
        private int _health;
        public int MaxHealth { get; set; }
        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                if (IsDead || value < 0)
                    _health = 0;
                else if (value > MaxHealth)
                    _health = MaxHealth;
                else
                    _health = value;
                if (_health == 0)
                    IsDead = true;
            }
        }
        public bool IsDead { get; set; }
        public bool IsNotDead { get { return !IsDead; } }

        public DamagableEntity(AbstractRoom<TRoom> spawnRoom) : base(spawnRoom)
        {

        }
    }
}
