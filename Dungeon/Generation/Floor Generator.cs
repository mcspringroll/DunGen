using System;
using DungeonAPI.Definitions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Generation
{
    public class Floor_Generator
    {
        private Floor floorBeingBuilt;
        private GenerationCommand[] allCommands;
        private FloorSprawler activeFloorSprawler;
        private int
            commandCount,
            roomsBuilt;

        public Floor_Generator(uint[] commands)
        {
            floorBeingBuilt = new Floor();
            allCommands = GenerationCommand.parseArray(commands);
            activeFloorSprawler = new FloorSprawler();
            //I'm assuming at some point the Room constructor or
            //Floor constructor will be changed so that Room properties can 
            //be easily set when constructing a new Room?
            activeFloorSprawler.addRoom(floorBeingBuilt.StartRoom);
            commandCount = roomsBuilt = 0;
            populateFloorWithRooms();
        }

        private void populateFloorWithRooms()
        {
            while (floorBeingBuilt.RoomCount > roomsBuilt) //RoomCount isn't initialized in Floor()
            {
                executeNextCommand();
                commandCount = (commandCount + 1) % allCommands.Length;
                //why would you need to execute more commands than you have? 
                //if RoomCount is bigger than the number of commands you have? 
                //how are you generating arrays of commands (assuming this is gonna be another class 
                //to generate a huge array of unsigned ints--some sort of command generator class?)
                //and can't you just always generate the right number of commands?
                //is this going to be randomly generated, or will you set certain commands for different
                //levels or floors? if so, probably need some sort of util class to store those arrays
            }
        }

        /// <summary>
        /// This is the heart of the floor generator, honestly.  As a result it
        /// is one beast of a method.  I'll go through step by step (steps are
        /// internally labeled as well):
        /// 
        /// 1. Get the next command, wrap around (in case we need to execute
        /// more commands than we have) is handled in populateFloorWithRooms
        /// where commandCount is set.
        /// 
        /// 2. Set activeFloorSprawler to be either DepthFirst or BreadthFirst
        /// depending on the command.  DepthFirst adds on to most recently
        /// added rooms and BreadthFirst adds on to least recently (oldest)
        /// rooms.
        /// 
        /// 3. Add the number of rooms required by this command.  This is a
        /// number between 1 and 16.  If a higher number of rooms is desired,
        /// just repeat the command.
        /// 
        /// 4. Retrieve the next room to be added to.  This room is guaranteeed
        /// to have at least one open side, though not necessarily any side
        /// that the command from step 1 will try to add to (issue fg01).
        /// 
        /// 5. Move the "next" room to the "end" of the floorsprawler if flag
        /// bit is active.  This effectively does nothing if the sprawl type is
        /// set to depth first.  This is also checked after each new room is
        /// created.
        /// 
        /// 6. Create an array of big enough size to contain any rooms that may
        /// be added in this pass.  Every index location is not guaranteed to
        /// be non-null.  This is because a room may be suggested to be added,
        ///  but there is another room at that location already.
        /// 
        /// 7. This attempts to build four rooms, one in each direction.  The 
        /// directions are attempted in the NESW order.  The factors that may 
        /// prevent a room from being built are as follows:
        ///     7a. Only build if the command calls for a room in the
        ///              direction.
        ///     7b. Only build if no room exists at the coordinates the new
        ///             room would be built in.
        ///     7c. Do not add if there was an issue connecting the two rooms.
        ///             Realistically, this should never actually restrict a
        ///             room from being built.
        /// 7. (continued) After a room has successfully been created, do the
        /// following:
        ///     7d. Add it to the array created in step 5.
        ///     7e. Finalize a room before moving to the next if command calls
        ///             for each "next" room to be "new."  If not, finalizing
        ///             is handled later (8b and 8c).
        /// 
        /// 8. Takes care of any non-directional checks and changes for rooms
        /// that are being added.  Only done if command does not call for
        /// each "next" room to be "new."  This includes:
        ///     8a. Shuffle the rooms in this pass before adding them to the
        ///                 sprawler.  
        ///     8b. Try to turn room into wall.  This is handled by a method.
        ///     8c. If the room was not turned into a wall, do the following:
        ///         8c1. Decrement the counter for the number of rooms left
        ///                 to be built.
        ///         8c2. Adds the room to the floor sprawler.
        /// 
        /// 9. Takes care of connecting rooms to their neighbors if the current
        /// command string calls for it. Some details follow:
        ///     9a. Only try a direction if the room does not already have a
        ///             connection in that direction.
        ///     9b. Get the room that is directly to the north of the current 
        ///             room (if one exists) and connect them.
        /// </summary>
        private void executeNextCommand()
        {
            // 1.
            GenerationCommand currentCommand = allCommands[commandCount];
            // 2.
            activeFloorSprawler.IsDepthFirst = currentCommand.isDepthFirst();
            // 3.
            int noRoomGeneratedCount = 0;
            while (currentCommand.roomsLeft() > 0 && noRoomGeneratedCount < 10)
            {
                // 4.
                Room currentRoom = activeFloorSprawler.getNextRoom();
                // 5.
                if (currentCommand.shouldAlwaysBranchFromNewRoom())
                {
                    activeFloorSprawler.removeNextRoom();
                    activeFloorSprawler.addRoom(currentRoom);
                }
                //so every time, you're removing NextRoom and then adding it back instantly. why?
                //I get that it might change the order (remove first node, add at end node) but otherwise...
                // 6.
                Room[] roomsInPass = new Room[currentCommand.roomsInSinglePass()];
                int roomsInArray = 0;
                // 7.
                if (currentCommand.shouldTryToBuildNorth() // 7a. Command to build north
                    && !floorBeingBuilt.hasRoomAtCoords(currentRoom.X, currentRoom.Y + 1) // 7b. No room exists to north
                    && (new Room()).setToNorthOf(currentRoom)) // 7c. Try to add room to the north
                    //I think (maybe) these last two checks are now redundant b/c we store all rooms
                    //around a room, connected or not, and setToNorthOf checks to see if currentRoom
                    //has a North room (so i think we can delete the middle condition)
                {
                    // 7d.
                    roomsInPass[roomsInArray] = currentRoom.North;
                    roomsInArray++;
                    // 7e.
                    if (currentCommand.shouldAlwaysBranchFromNewRoom())
                    {
                        roomsInPass[roomsInArray].IsWall = tryForWallRoom(currentCommand);
                        if (roomsInPass[roomsInArray].IsNotWall)
                        {
                            currentCommand.decrementRoomsLeft();
                            activeFloorSprawler.addRoom(roomsInPass[roomsInArray]);
                        }
                        currentRoom = activeFloorSprawler.getNextRoom();
                        activeFloorSprawler.removeNextRoom();
                        //okay i thought i was maybe starting to understand shouldAlwaysBranchFromNewRoom
                        //but now I am even more confused. didn't you just...remove the current room from
                        //the floor sprawler?
                    }
                }
                if (currentCommand.shouldTryToBuildEast()
                    && !floorBeingBuilt.hasRoomAtCoords(currentRoom.X + 1, currentRoom.Y)
                    && (new Room()).setToEastOf(currentRoom))
                {
                    roomsInPass[roomsInArray] = currentRoom.East;
                    roomsInArray++;

                    if (currentCommand.shouldAlwaysBranchFromNewRoom())
                    {
                        roomsInPass[roomsInArray].IsWall = tryForWallRoom(currentCommand);
                        if (roomsInPass[roomsInArray].IsNotWall)
                        {
                            currentCommand.decrementRoomsLeft();
                            activeFloorSprawler.addRoom(roomsInPass[roomsInArray]);
                        }
                        currentRoom = activeFloorSprawler.getNextRoom();
                        activeFloorSprawler.removeNextRoom();
                    }
                }
                if (currentCommand.shouldTryToBuildSouth()
                    && !floorBeingBuilt.hasRoomAtCoords(currentRoom.X, currentRoom.Y - 1)
                    && (new Room()).setToSouthOf(currentRoom))
                {
                    roomsInPass[roomsInArray] = currentRoom.South;
                    roomsInArray++;

                    if (currentCommand.shouldAlwaysBranchFromNewRoom())
                    {
                        roomsInPass[roomsInArray].IsWall = tryForWallRoom(currentCommand);
                        if (roomsInPass[roomsInArray].IsNotWall)
                        {
                            currentCommand.decrementRoomsLeft();
                            activeFloorSprawler.addRoom(roomsInPass[roomsInArray]);
                        }
                        currentRoom = activeFloorSprawler.getNextRoom();
                        activeFloorSprawler.removeNextRoom();
                    }
                }
                if (currentCommand.shouldTryToBuildWest()
                    && !floorBeingBuilt.hasRoomAtCoords(currentRoom.X - 1, currentRoom.Y)
                    && (new Room()).setToWestOf(currentRoom))
                {
                    roomsInPass[roomsInArray] = currentRoom.West;
                    roomsInArray++;

                    if (currentCommand.shouldAlwaysBranchFromNewRoom())
                    {
                        roomsInPass[roomsInArray].IsWall = tryForWallRoom(currentCommand);
                        if (roomsInPass[roomsInArray].IsNotWall)
                        {
                            currentCommand.decrementRoomsLeft();
                            activeFloorSprawler.addRoom(roomsInPass[roomsInArray]);
                        }
                        currentRoom = activeFloorSprawler.getNextRoom();
                        activeFloorSprawler.removeNextRoom();
                    }
                }
                // 8.
                if (!currentCommand.shouldAlwaysBranchFromNewRoom())
                {
                    // 8a.
                    if (currentCommand.shouldShuffleThisPass())
                    {
                        roomsInPass = shuffleRoomArray(roomsInPass);
                    }
                    for (int i = 0; i < roomsInArray; i++)
                    {
                        // 8b.
                        roomsInPass[i].IsWall = tryForWallRoom(currentCommand);
                        // 8c.
                        if (roomsInPass[i].IsNotWall)
                        {
                            // 8c1.
                            currentCommand.decrementRoomsLeft();
                            // 8c2.
                            activeFloorSprawler.addRoom(roomsInPass[i]);
                        }
                    }   
                }

                // 9.
                if(currentCommand.shouldConnectNeighbors())
                {
                    for(int i = 0; i < roomsInArray; i ++)
                    {
                        // 9a.
                        if (roomsInPass[i].North == null)
                        {
                            // 9b.
                            Room northNeighbor = floorBeingBuilt.getRoomToNorth(roomsInPass[i]);
                            if (northNeighbor != null)
                            {
                                northNeighbor.setToNorthOf(roomsInPass[i]);
                            }
                        }
                        if (roomsInPass[i].East == null)
                        {
                            Room northNeighbor = floorBeingBuilt.getRoomToEast(roomsInPass[i]);
                            if (northNeighbor != null)
                            {
                                northNeighbor.setToEastOf(roomsInPass[i]);
                            }
                        }
                        if (roomsInPass[i].South == null)
                        {
                            Room northNeighbor = floorBeingBuilt.getRoomToSouth(roomsInPass[i]);
                            if (northNeighbor != null)
                            {
                                northNeighbor.setToSouthOf(roomsInPass[i]);
                            }
                        }
                        if (roomsInPass[i].West == null)
                        {
                            Room northNeighbor = floorBeingBuilt.getRoomToWest(roomsInPass[i]);
                            if (northNeighbor != null)
                            {
                                northNeighbor.setToWestOf(roomsInPass[i]);
                            }
                        }
                    }
                }
            }
        }

        private Room[] shuffleRoomArray(Room[] input)
        {
            FloorSprawler temp = new FloorSprawler();
            foreach(Room r in input)
            {
                if(r != null)
                {
                    temp.addRoom(r);
                }
            }
            temp.shuffleRooms();
            Room[] toReturn = new Room[temp.NumberOfRooms];
            for(int i = 0; i < toReturn.Length; i++)
            {
                toReturn[i] = temp.removeNextRoom();
            }
            return null;
        }

        private bool tryForWallRoom(GenerationCommand command)
        {
            return ((new Random()).NextDouble() < command.getWallChance());
        }
    }
}
