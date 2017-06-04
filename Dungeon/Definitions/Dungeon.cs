namespace DungeonAPI.Definitions
{
    public class Dungeon <TRoom> where TRoom : AbstractRoom<TRoom>, new()
    {
        public Floor<TRoom> CurrentFloor
        {
            get; private set;
        }

        public Player<TRoom> CurrentPlayer
        {
            get; private set;
        }

        public Dungeon()
        {
            CurrentFloor = new Floor<TRoom>();
            CurrentPlayer = new Player<TRoom>(CurrentFloor.CurrentRoom);
        }
    }
}
