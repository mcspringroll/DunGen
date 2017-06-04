namespace DungeonAPI.Definitions
{
    public class Obstacle<TRoom> : PhysicalEntity<TRoom> where TRoom : AbstractRoom<TRoom>, new()
    {
        public Obstacle(AbstractRoom<TRoom> spawnRoom) : base(spawnRoom)
        {
        }
        public bool BlocksVision { get; set; }
        public bool BlocksAllMovement { get; set; }
        public bool BlocksGroundMovement { get; set; }
        public bool Breakable { get; set; }
        public bool Movable { get; set; }
    }
}
