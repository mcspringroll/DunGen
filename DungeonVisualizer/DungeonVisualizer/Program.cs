using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonAPI.Generation;
using DungeonAPI.Definitions;

namespace DungeonVisualizer
{
    class Program
    {
        static void Main(string[] args)
        {
            uint[] buildCommands = new uint[1];
            //buildCommands[0] = 0x28008000;
            //buildCommands[1] = 0x22008000;
            //buildCommands[2] = 0x44008000;


            buildCommands[0] = 0xFFFFC400;


            //buildCommands[0] = 0xFF000000;
            FloorGenerator generator = new FloorGenerator(buildCommands, 500);
            Floor floor = generator.GetFloor();
            Room[,] rooms = SendFloorToArray(floor);
            PrintRooms(rooms);
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

            foreach(Room r in floorToConvert.GetRooms())
            {
                rooms[height - 1 - (r.Y - smallestY), r.X - smallestX] = r;
            }

            return rooms;
        }

        private static void PrintRooms(Room[,] rooms)
        {
            Console.SetWindowSize(200, 60);
            Console.WriteLine();
            for (int row = 0; row < rooms.GetLength(0); row++)
            {
                for (int col = 0; col < rooms.GetLength(1); col++)
                {
                    if (rooms[row, col] == null || rooms[row, col].IsWall)
                        Console.Write("   ");
                    else
                        Console.Write("[ ]");
                }
                Console.Write('\n');
            }
        }
    }
}
