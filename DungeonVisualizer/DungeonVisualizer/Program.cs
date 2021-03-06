﻿using System;
using DungeonAPI.Generation;
using DungeonAPI.Definitions;
using DungeonAPI.Exceptions;

namespace DungeonVisualizer
{
    class Program
    {
        static void Main(string[] args)
        {
            uint[] buildCommands = new uint[1];
            /*buildCommands[0] = 0x28008000;
            buildCommands[1] = 0x22008000;
            buildCommands[2] = 0x44008000;*/


            //buildCommands[0] = 0xFFFFC400;


            //buildCommands[0] = 0xFC000400;

            // buildCommands[0] = 4227859456;

            //buildCommands[0] = 0x48008000;
            //buildCommands[1] = 0x2FF04400;
            //buildCommands[2] = 0x42008000;
            //buildCommands[3] = 0x2FF04400;
            //buildCommands[4] = 0x42008000;
            //buildCommands[5] = 0x2FF04400;
            //buildCommands[6] = 0x41008000;
            //buildCommands[7] = 0x2FF06400;

            buildCommands[0] = 0x1FE5ffff;

            FloorGenerator<Room> generator = new FloorGenerator<Room>(buildCommands, 1000, 2, true);
            bool noSuccess = true;
            int timeElapsed = -1;

            while (noSuccess)
            {
                DateTime startTime = DateTime.Now;
                try
                {
                    generator.GenerateFloor();

                    timeElapsed = GetTimeElapsed(startTime, DateTime.Now);

                    noSuccess = false;
                }
                catch (NoRoomsToGenerateFromException)
                {
                    timeElapsed= GetTimeElapsed(startTime, DateTime.Now);

                    Console.WriteLine("Generation stopped because no more rooms were able to be added.");
                    Floor<Room> failedFloor = generator.GetFloor();
                    Room[,] roomsOfFailedFloor = SendFloorToArray(failedFloor);
                    PrintRooms(roomsOfFailedFloor, failedFloor.RoomCount, buildCommands, timeElapsed, false);
                    generator.Reset();
                }
            }
            Console.WriteLine("Success!");
            Floor<Room> floor = generator.GetFloor();
            Room[,] rooms = SendFloorToArray(floor);
            PrintRooms(rooms, floor.RoomCount, buildCommands, timeElapsed, true);
        }

        private static Room[,] SendFloorToArray(Floor<Room> floorToConvert)
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

        private static void PrintRooms(Room[,] rooms, int numberOfRooms, uint[] commands, int timeElapsed, bool success)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location + "\\..\\Rooms";
            try
            {
                System.IO.Directory.CreateDirectory(path);
            }
            catch (Exception) { }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter((path + "\\" + (success ? "successful" : "unsuccessful") + "Floor" + RandomUtility.RandomPositiveInt() + ".txt")))
            {
                file.WriteLine("This floor was created in " + timeElapsed + " seconds and has " + numberOfRooms + " rooms (counting walls) and was created " + (success ? "successfully" : "unsuccessfully") + " for this sequence of commands:\n");
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

        private static int GetTimeElapsed(DateTime start, DateTime end)
        {
            return (int)(end - start).TotalSeconds;
        }
    }
}
