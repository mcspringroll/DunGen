namespace DungeonAPI.Definitions
{
    public class Player<TRoom> : FightingEntity<TRoom> where TRoom : AbstractRoom<TRoom>, new()
    {
        public Player(AbstractRoom<TRoom> spawnRoom) : base(spawnRoom)
        {
            MaxHealth = 3;
            Health = MaxHealth;
        }
    }
}
