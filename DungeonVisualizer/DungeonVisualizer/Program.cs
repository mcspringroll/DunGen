using System;
using DungeonAPI.Generation;
using DungeonAPI.Definitions;
using DungeonAPI.Exceptions;

namespace DungeonVisualizer
{
    class Program
    {
        static void Main(string[] args)
        {
            uint[] buildCommands = new uint[8];
            /*buildCommands[0] = 0x28008000;
            buildCommands[1] = 0x22008000;
            buildCommands[2] = 0x44008000;*/


            //buildCommands[0] = 0xFFFFC400;


            //buildCommands[0] = 0xFF000000;


            buildCommands[0] = 0x48008000;
            buildCommands[1] = 0x2FF04400;
            buildCommands[2] = 0x42008000;
            buildCommands[3] = 0x2FF04400;
            buildCommands[4] = 0x42008000;
            buildCommands[5] = 0x2FF04400;
            buildCommands[6] = 0x41008000;
            buildCommands[7] = 0x2FF06400;

            FloorGenerator generator = new FloorGenerator(buildCommands, 1000000, 1, true);
            bool noSuccess = true;
            while (noSuccess)
            {
                try
                {
                    generator.GenerateFloor();
                    noSuccess = false;
                }
                catch (NoRoomsToGenerateFromException)
                {
                    Console.WriteLine("Generation stopped because no more rooms were able to be added.");
                    Floor failedFloor = generator.GetFloor();
                    Room[,] roomsOfFailedFloor = SendFloorToArray(failedFloor);
                    PrintRooms(roomsOfFailedFloor, failedFloor.RoomCount, buildCommands, false);
                    generator.Reset();
                }
            }
            Console.WriteLine("Success!");
            Floor floor = generator.GetFloor();
            Room[,] rooms = SendFloorToArray(floor);
            PrintRooms(rooms, floor.RoomCount, buildCommands, true);
        }

        private static Room[,] SendFloorToArray(Floor floorToConvert)
        {
            int smallestX = floorToConvert.SmallestX,
                    smallestY = floorToConvert.SmallestY,
                    largestX = floorToConvert.LargestX,
                    largestY = floorToConvert.LargestY,
                    width = floorToConvert.Width,
                    height = floorToConvert.Height;

            Room[,] rooms = new Room[height, width];

            foreach (Room r in floorToConvert.GetRooms())
            {
                rooms[height - 1 - (r.Y - smallestY), r.X - smallestX] = r;
            }

            return rooms;
        }

        private static void PrintRooms(Room[,] rooms, int numberOfRooms, uint[] commands, bool success)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location + "\\..\\Rooms";
            try
            {
                System.IO.Directory.CreateDirectory(path);
            }
            catch (Exception) { }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter((path + "\\" + (success ? "successful" : "unsuccessful") + "Room" + RandomUtility.RandomPositiveInt() + ".txt")))
            {
                file.WriteLine("This floor has " + numberOfRooms + " rooms (counting walls) and was created " + (success ? "successfully" : "unsuccessfully") + " for this sequence of commands:\n");
                foreach (uint command in commands)
                    file.WriteLine(command);
                file.WriteLine("\n\n\n\n");
                for (int row = 0; row < rooms.GetLength(0); row++)
                {
                    for (int col = 0; col < rooms.GetLength(1); col++)
                    {
                        if (rooms[row, col] == null)
                            file.Write("  ");
                        else if (rooms[row, col].IsWall)
                            file.Write("\u2591\u2591");
                        else
                            file.Write("\u2588\u2588");
                    }
                    file.Write('\n');
                }
            }
        }
    }
}
