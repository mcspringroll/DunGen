namespace DungeonAPI.Definitions
{
    public class Room : AbstractRoom <Room>
    {
        public Room() : this(0,0)
        { }

        public Room(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
