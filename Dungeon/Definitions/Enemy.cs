namespace DungeonAPI.Definitions
{
    public class Enemy <TRoom> : FightingEntity<TRoom> where TRoom : AbstractRoom<TRoom>, new()
    {
        public Enemy(TRoom spawnRoom) : base(spawnRoom)
        {

        }
    }
}
