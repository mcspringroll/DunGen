namespace DungeonAPI.Definitions
{
    public abstract class PhysicalEntity<TRoom> where TRoom : AbstractRoom<TRoom>, new()
    {
        public int XLocation { get; set; }
        public int YLocation { get; set; }

        public AbstractRoom<TRoom> CurrentRoom { get; set; }
        
        public void move(int moveX, int moveY)
        {
            XLocation += moveX;
            YLocation += moveY;
        }
        
        public PhysicalEntity(AbstractRoom<TRoom> spawnRoom)
        {
            CurrentRoom = spawnRoom;
            XLocation = 0;
            YLocation = 0;
        }
    }
}
