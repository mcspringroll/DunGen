using System;
using DungeonAPI.Definitions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Generation
{
    public class FloorGenerator
    {
        private const int MAXIMUM_GENERATION_FAILS_BEFORE_COMMAND_IS_SKIPPED = 50;

        private Floor floorBeingBuilt;
        private GenerationCommand[] allCommands;
        private GenerationCommand currentCommand;
        private FloorSprawler activeFloorSprawler;
        private int
            commandCount,
            roomsBuilt;

        public FloorGenerator(uint[] commands)
        {
            floorBeingBuilt = new Floor();
            allCommands = GenerationCommand.parseArray(commands);
            activeFloorSprawler = new FloorSprawler();
            activeFloorSprawler.addRoom(floorBeingBuilt.StartRoom);
            commandCount = roomsBuilt = 0;
            PopulateFloorWithRooms();
        }

        /// <summary>
        /// Executes commands in order until the floor has the
        /// appropriate number of rooms.
        /// </summary>
        private void PopulateFloorWithRooms()
        {
            while (floorBeingBuilt.RoomCount > roomsBuilt)
            {
                ExecuteNextCommand();
            }
        }

        /// <summary>
        /// Triggers all of the generation based on the
        /// next command in allCommands.
        /// </summary>
        private void ExecuteNextCommand()
        {
            UpdateCurrentCommand();
            UpdateFloorSprawlerGrowthPattern();
            bool successfullyExecuted = ExecuteCommandLoop();

            if(!successfullyExecuted)
            {
                //We should do something here.  Because if this was triggered,
                //there was a lot of failed attempts.  Potentially re-add all
                //rooms to the sprawler.  Or clear some walls or something.
            }
        }

        /// <summary>
        /// Updates the currentCommand instance variable that is used
        /// by most other methods.  Also updates the commandCount instance
        /// variable that specifies what is the next command.  If this
        /// count excedes the number of commands available, it will wrap
        /// to zero and start the cycle over again.
        /// </summary>
        private void UpdateCurrentCommand()
        {
            currentCommand = allCommands[commandCount];
            commandCount = (commandCount + 1) % allCommands.Length;
        }

        /// <summary>
        /// Set activeFloorSprawler to be either DepthFirst or BreadthFirst.
        /// DepthFirst adds on to most recently added rooms and 
        /// BreadthFirst adds on to least recently (oldest) rooms.
        /// </summary>
        private void UpdateFloorSprawlerGrowthPattern()
        {
            activeFloorSprawler.IsDepthFirst = currentCommand.isDepthFirst();
        }

        /// <summary>
        /// Executes the command as many times the command demands.  If
        /// the command unsuccessfully adds rooms too many times in a row,
        /// it will terminate early.
        /// </summary>
        /// <returns>
        /// True if the command executed successfully otherwise false if
        /// it was terminated early because it reached the max number of
        /// failed generations.
        /// </returns>
        private bool ExecuteCommandLoop()
        {
            int failedRoomGenerationAttempts = 0;
            while (currentCommand.roomsLeft() > 0 && failedRoomGenerationAttempts < MAXIMUM_GENERATION_FAILS_BEFORE_COMMAND_IS_SKIPPED)
            {
                int roomsGenerated = ExecuteCommandOnce();
                roomsBuilt += roomsGenerated;
                if (roomsGenerated == 0)
                {
                    failedRoomGenerationAttempts++;
                }
            }

            return failedRoomGenerationAttempts < MAXIMUM_GENERATION_FAILS_BEFORE_COMMAND_IS_SKIPPED;
        }

        /// <summary>
        /// Performs a single execution or pass of
        /// currentCommand.
        /// </summary>
        /// <returns>The number of non-wall rooms added
        /// by the pass.</returns>
        private int ExecuteCommandOnce()
        {
            Room roomToBuildFrom = GetRoomToBuildOnFromSprawler();

            List<Room> roomsInPass = BuildRoomsInPass(roomToBuildFrom);
            
            if (currentCommand.ShouldNotAlwaysBranchFromNewRoom())
            {
                roomsInPass = TryToShuffleRoomsInPass(roomsInPass);
                FinalizeRoomsInPass(roomsInPass);
            }
            
            if (currentCommand.shouldConnectNeighbors())
            {
                ConnectNeighbors(roomsInPass);
            }
            int roomsAdded = CountRoomsThatAreNotWalls(roomsInPass);
            return roomsAdded;
        }

        /// <summary>
        /// Counts the number of rooms in roomsInPass that
        /// are not walls.
        /// </summary>
        /// <param name="roomsInPass"></param>
        /// <returns>The number that aren't walls.</returns>
        private int CountRoomsThatAreNotWalls(List<Room> roomsInPass)
        {
            int roomsThatAreNotWalls = 0;
            foreach(Room room in roomsInPass)
            {
                if (room.IsNotWall)
                    roomsThatAreNotWalls++;
            }
            return roomsThatAreNotWalls;
        }

        /// <summary>
        /// Connects rooms that are adjacent to a room in
        /// roomsInPass to that room.
        /// </summary>
        /// <param name="roomsInPass"></param>
        private void ConnectNeighbors(List<Room> roomsInPass)
        {
            foreach (Room room in roomsInPass)
            {
                TryToConnectToNorth(room);
                TryToConnectToEast(room);
                TryToConnectToSouth(room);
                TryToConnectToWest(room);
            }
        }

        /// <summary>
        /// Attempts to connect roomToConnectFrom to the room directly to
        /// its north.  If no such room exists, no connection is made.  If
        /// roomToConnectFrom is already connected to the north, nothing
        /// is changed.
        /// </summary>
        /// <param name="roomToConnectFrom"></param>
        private void TryToConnectToNorth(Room roomToConnectFrom)
        {
            if (roomToConnectFrom.North == null)
            {
                Room northNeighbor = floorBeingBuilt.getRoomToNorth(roomToConnectFrom);
                if (northNeighbor != null)
                {
                    northNeighbor.setToNorthOf(roomToConnectFrom);
                }
            }
        }

        /// <summary>
        /// Attempts to connect roomToConnectFrom to the room directly to
        /// its east.  If no such room exists, no connection is made.  If
        /// roomToConnectFrom is already connected to the east, nothing
        /// is changed.
        /// </summary>
        /// <param name="roomToConnectFrom"></param>
        private void TryToConnectToEast(Room roomToConnectFrom)
        {
            if (roomToConnectFrom.East == null)
            {
                Room eastNeighbor = floorBeingBuilt.getRoomToEast(roomToConnectFrom);
                if (eastNeighbor != null)
                {
                    eastNeighbor.setToEastOf(roomToConnectFrom);
                }
            }
        }

        /// <summary>
        /// Attempts to connect roomToConnectFrom to the room directly to
        /// its south.  If no such room exists, no connection is made.  If
        /// roomToConnectFrom is already connected to the south, nothing
        /// is changed.
        /// </summary>
        /// <param name="roomToConnectFrom"></param>
        private void TryToConnectToSouth(Room roomToConnectFrom)
        {
            if (roomToConnectFrom.South == null)
            {
                Room southNeighbor = floorBeingBuilt.getRoomToSouth(roomToConnectFrom);
                if (southNeighbor != null)
                {
                    southNeighbor.setToSouthOf(roomToConnectFrom);
                }
            }
        }

        /// <summary>
        /// Attempts to connect roomToConnectFrom to the room directly to
        /// its west.  If no such room exists, no connection is made.  If
        /// roomToConnectFrom is already connected to the west, nothing
        /// is changed.
        /// </summary>
        /// <param name="roomToConnectFrom"></param>
        private void TryToConnectToWest(Room roomToConnectFrom)
        {
            if (roomToConnectFrom.East == null)
            {
                Room westNeighbor = floorBeingBuilt.getRoomToWest(roomToConnectFrom);
                if (westNeighbor != null)
                {
                    westNeighbor.setToWestOf(roomToConnectFrom);
                }
            }
        }

        /// <summary>
        /// Finalizes every Room in roomsInPass List.
        /// </summary>
        /// <param name="roomsInPass"></param>
        private void FinalizeRoomsInPass(List<Room> roomsInPass)
        {
            foreach (Room room in roomsInPass)
            {
                FinalizeAddingRoom(room);
            }
        }

        /// <summary>
        /// If currentCommand demands that passes should be shuffled,
        /// it will reate a new List of type Room that contains the same
        /// Room objects as roomsInPass.
        /// </summary>
        /// <param name="roomsInPass"></param>
        /// <returns>A List of type Room in a different order than roomsInPass
        /// if currentCommand specifies that passes should be shuffled.
        /// Otherwise, return roomsInPass.</returns>
        private List<Room> TryToShuffleRoomsInPass(List<Room> roomsInPass)
        {
            if (currentCommand.shouldShuffleThisPass())
            {
                return GetShuffledRoomArray(roomsInPass);
            }

            return roomsInPass;
        }

        /// <summary>
        /// Generates appropriates rooms according to the
        /// specifications of currentCommand.  This can result
        /// in a maximum of four rooms being generated.  Also
        /// uses the currentCommand to potentially turn some
        /// or all of those rooms into wall pieces.  Rooms
        /// that are turned into wall pieces are not added to
        /// the floor sprawler, all others are.
        /// </summary>
        /// <param name="roomToBuildFrom"></param>
        /// <returns>A List container of the rooms added to the
        /// floor during this method call.  This includes wall
        /// pieces.</returns>
        private List<Room> BuildRoomsInPass(Room roomToBuildFrom)
        {
            List<Room> roomsToReturn = new List<Room>();

            if (currentCommand.shouldTryToBuildNorth())
            {
                Room northRoom = BuildNorthRoom(roomToBuildFrom);
                if (northRoom != null)
                {
                    if (currentCommand.shouldAlwaysBranchFromNewRoom())
                    {
                        FinalizeAddingRoom(northRoom);
                        roomToBuildFrom = GetRoomToBuildOnFromSprawler();
                    }
                    roomsToReturn.Add(northRoom);
                }
            }

            if (currentCommand.shouldTryToBuildEast())
            {
                Room eastRoom = BuildEastRoom(roomToBuildFrom);
                if (eastRoom != null)
                {
                    if (currentCommand.shouldAlwaysBranchFromNewRoom())
                    {
                        FinalizeAddingRoom(eastRoom);
                        roomToBuildFrom = GetRoomToBuildOnFromSprawler();
                    }
                    roomsToReturn.Add(eastRoom);
                }
            }

            if (currentCommand.shouldTryToBuildSouth())
            {
                Room southRoom = BuildSouthRoom(roomToBuildFrom);
                if (southRoom != null)
                {
                    if (currentCommand.shouldAlwaysBranchFromNewRoom())
                    {
                        FinalizeAddingRoom(southRoom);
                        roomToBuildFrom = GetRoomToBuildOnFromSprawler();
                    }
                    roomsToReturn.Add(southRoom);
                }
            }

            if (currentCommand.shouldTryToBuildWest())
            {
                Room westRoom = BuildWestRoom(roomToBuildFrom);
                if (westRoom != null)
                {
                    if (currentCommand.shouldAlwaysBranchFromNewRoom())
                    {
                        FinalizeAddingRoom(westRoom);
                        roomToBuildFrom = GetRoomToBuildOnFromSprawler();
                    }
                    roomsToReturn.Add(westRoom);
                }
            }

            return roomsToReturn;
        }

        /// <summary>
        /// Potentially turns this room into a wall based
        /// on specification from currentCommand.  Rooms
        /// that are not turned into walls are added to
        /// activeFloorSprawler and are counted toward the
        /// number of rooms created by currentCommand.
        /// </summary>
        /// <param name="roomToFinalize"></param>
        private void FinalizeAddingRoom(Room roomToFinalize)
        {
            roomToFinalize.IsWall = TryForWallRoom();
            if (roomToFinalize.IsNotWall)
            {
                currentCommand.decrementRoomsLeft();
                activeFloorSprawler.addRoom(roomToFinalize);
            }
        }

        /// <summary>
        /// Will attempt to create a new room to the north of 
        /// the roomToBuildFrom.  If a room already exists at
        /// the coordinate to the north of roomToBuildFrom,
        /// no room will be added.
        /// </summary>
        /// <param name="roomToBuildFrom"></param>
        /// <returns>A room connected to the north of
        /// roomtoBuildFrom if one does not exist at that location.
        /// Otherwise, returns null.</returns>
        private Room BuildNorthRoom(Room roomToBuildFrom)
        {
            Room northRoom = null;
            bool connectionWasSuccessful = false;

            if (floorBeingBuilt.doesNotHaveRoomAtCoords(roomToBuildFrom.X, roomToBuildFrom.Y + 1))
            {
                northRoom = new Room();
                connectionWasSuccessful = northRoom.setToNorthOf(roomToBuildFrom);
            }
            if (connectionWasSuccessful)
                return northRoom;
            else
                return null;
        }

        /// <summary>
        /// Will attempt to create a new room to the east of 
        /// the roomToBuildFrom.  If a room already exists at
        /// the coordinate to the east of roomToBuildFrom,
        /// no room will be added.
        /// </summary>
        /// <param name="roomToBuildFrom"></param>
        /// <returns>A room connected to the east of
        /// roomtoBuildFrom if one does not exist at that location.
        /// Otherwise, returns null.</returns>
        private Room BuildEastRoom(Room roomToBuildFrom)
        {
            Room eastRoom = null;
            bool connectionWasSuccessful = false;

            if (floorBeingBuilt.doesNotHaveRoomAtCoords(roomToBuildFrom.X + 1, roomToBuildFrom.Y))
            {
                eastRoom = new Room();
                connectionWasSuccessful = eastRoom.setToEastOf(roomToBuildFrom);
            }
            if (connectionWasSuccessful)
                return eastRoom;
            else
                return null;
        }

        /// <summary>
        /// Will attempt to create a new room to the south of 
        /// the roomToBuildFrom.  If a room already exists at
        /// the coordinate to the south of roomToBuildFrom,
        /// no room will be added.
        /// </summary>
        /// <param name="roomToBuildFrom"></param>
        /// <returns>A room connected to the south of
        /// roomtoBuildFrom if one does not exist at that location.
        /// Otherwise, returns null.</returns>
        private Room BuildSouthRoom(Room roomToBuildFrom)
        {
            Room southRoom = null;
            bool connectionWasSuccessful = false;

            if (floorBeingBuilt.doesNotHaveRoomAtCoords(roomToBuildFrom.X, roomToBuildFrom.Y - 1))
            {
                southRoom = new Room();
                connectionWasSuccessful = southRoom.setToSouthOf(roomToBuildFrom);
            }
            if (connectionWasSuccessful)
                return southRoom;
            else
                return null;
        }

        /// <summary>
        /// Will attempt to create a new room to the west of 
        /// the roomToBuildFrom.  If a room already exists at
        /// the coordinate to the west of roomToBuildFrom,
        /// no room will be added.
        /// </summary>
        /// <param name="roomToBuildFrom"></param>
        /// <returns>A room connected to the west of
        /// roomtoBuildFrom if one does not exist at that location.
        /// Otherwise, returns null.</returns>
        private Room BuildWestRoom(Room roomToBuildFrom)
        {
            Room westRoom = null;
            bool connectionWasSuccessful = false;

            if (floorBeingBuilt.doesNotHaveRoomAtCoords(roomToBuildFrom.X - 1, roomToBuildFrom.Y))
            {
                westRoom = new Room();
                connectionWasSuccessful = westRoom.setToWestOf(roomToBuildFrom);
            }
            if (connectionWasSuccessful)
                return westRoom;
            else
                return null;
        }

        /// <summary>
        /// Retrieve the next room to be added to.  This room is guaranteeed
        /// to have at least one open side, but not guaranteed that the current
        /// command will try to add onto.  If the current command demands that
        /// all the floorsprawler always branches from a new room, it will move 
        /// the selected room to the end of the floorsprawler.  When the sprawl
        /// type is depth first this feature essentially does nothing.
        /// </summary>
        /// <returns>The room to build off from.</returns>
        private Room GetRoomToBuildOnFromSprawler()
        {
            Room roomToReturn = activeFloorSprawler.getNextRoom();

            if (currentCommand.shouldAlwaysBranchFromNewRoom())
            {
                activeFloorSprawler.removeNextRoom();
                activeFloorSprawler.addRoom(roomToReturn);
            }
            return roomToReturn;
        }

        /// <summary>
        /// Changes the order of input by adding the Room
        /// objects to a temporary FloorSprawler and using
        /// the shuffleRooms method of that class.  Returns
        /// as a new object.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>A List of type Room where the order is
        /// different than the order within input.</returns>
        private List<Room> GetShuffledRoomArray(List<Room> input)
        {
            FloorSprawler tempSprawler = new FloorSprawler();
            foreach (Room r in input)
            {
                if (r != null)
                {
                    tempSprawler.addRoom(r);
                }
            }
            tempSprawler.shuffleRooms();
            List<Room> toReturn = new List<Room>();
            while(tempSprawler.HasNextRoom)
            {
                toReturn.Add(tempSprawler.removeNextRoom());
            }
            return toReturn;
        }

        /// <summary>
        /// Returns true with the percentage specified by
        /// currentCommand.
        /// </summary>
        /// <returns></returns>
        private bool TryForWallRoom()
        {
            return ((new Random()).NextDouble() < currentCommand.getWallChance());
        }
    }
}
