﻿using System;
using DungeonAPI.Definitions;
using System.Collections.Generic;

namespace DungeonAPI.Generation
{
    /// <summary>
    /// Represents a single segment of generation command.  This includes
    /// how many rooms should be generated by this segment, which directions
    /// should be generated by this segment, the likelihood of a wall being
    /// generated by this segment, and all other generation modifiers for
    /// this segment.
    /// </summary>
    public sealed class GenerationCommand
    {
        protected const byte
            MASK_REPETITIONS = 0xf0, //1111 0000
            MASK_DIRECTIONS = 0x0f, //0000 1111
            MASK_NORTH = 0x08, //0000 1000
            MASK_EAST = 0x04, //0000 0100
            MASK_SOUTH = 0x02, //0000 0010
            MASK_WEST = 0x01, //0000 0001
            MASK_CONTROLLER_DEPTH_FIRST = 0x80, //1000 0000
            MASK_CONTROLLER_SHUFFLE = 0x40, //0100 0000
            MASK_CONTROLLER_SHUFFLE_ROOMS = 0x20, //0010 0000
            MASK_CONTROLLER_TOGGLE_DEPTH_FIRST = 0x10, //0001 0000
            MASK_CONTROLLER_CONNECT_NEIGHBORS = 0x08,  //0000 1000
            MASK_CONTROLLER_ALWAYS_NEW_ROOM = 0x04, //0000 0100
            MASK_CONTROLLER_MSI = 0x00, //???
            MASK_CONTROLLER_LSI = 0x00, //???
            MASK_CONTROLLER_LSB = 0x00; //???

        private byte 
            repetitionsAndDirections,
            wallChance,
            controllerMSB,
            controllerLSB;

        private bool roomsDepleted;

        private byte RoomsLeft { get; set; }

        public GenerationCommand(uint segmentValue)
        {
            byte[] segmentBytes = toByteArray(segmentValue);
            repetitionsAndDirections = segmentBytes[0];
            wallChance = segmentBytes[1];
            controllerMSB = segmentBytes[2];
            controllerLSB = segmentBytes[3];
            resetRoomsLeft();
        }

        /// <summary>
        /// Controls type of sprawl.
        /// </summary>
        /// <returns></returns>
        public bool isDepthFirst()
        {
            return (controllerMSB & MASK_CONTROLLER_DEPTH_FIRST) == MASK_CONTROLLER_DEPTH_FIRST;
        }

        /// <summary>
        /// Controls type of sprawl.
        /// </summary>
        /// <returns></returns>
        public bool isBreadthFirst()
        {
            return !isDepthFirst();
        }

        /// <summary>
        /// Get, remove, and add rooms to end everytime, not just at end of pass.
        /// </summary>
        /// <returns></returns>
        public bool shouldAlwaysBranchFromNewRoom()
        {
            return (controllerMSB & MASK_CONTROLLER_ALWAYS_NEW_ROOM) == MASK_CONTROLLER_ALWAYS_NEW_ROOM;
        }

        /// <summary>
        /// Randomize all stored rooms in sprawler.
        /// </summary>
        /// <returns></returns>
        public bool shuffleAfterPass()
        {
            return (controllerMSB & MASK_CONTROLLER_SHUFFLE) == MASK_CONTROLLER_SHUFFLE;
        }

        /// <summary>
        /// Randomize rooms before adding them to sprawler.
        /// </summary>
        /// <returns></returns>
        public bool shouldShuffleThisPass()
        {
            return (controllerMSB & MASK_CONTROLLER_SHUFFLE_ROOMS) == MASK_CONTROLLER_SHUFFLE_ROOMS;
        }

        /// <summary>
        /// Switch between sprawl type after a pass is completed.
        /// </summary>
        /// <returns></returns>
        public bool flipDepthOrBreadthFirstAfterPass()
        {
            return (controllerMSB & MASK_CONTROLLER_TOGGLE_DEPTH_FIRST) == MASK_CONTROLLER_TOGGLE_DEPTH_FIRST;
        }

        /// <summary>
        /// Controls if adjacent rooms should be connected.  If
        /// this returns false, then new rooms only connect to
        /// the room that branched to create them.
        /// </summary>
        /// <returns></returns>
        public bool shouldConnectNeighbors()
        {
            return (controllerMSB & MASK_CONTROLLER_CONNECT_NEIGHBORS) == MASK_CONTROLLER_CONNECT_NEIGHBORS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int roomsLeft()
        {
            return (int)RoomsLeft;
        }

        /// <summary>
        /// If this command should allow trying to build to the north.
        /// </summary>
        /// <returns></returns>
        public bool shouldTryToBuildNorth()
        {
            return (repetitionsAndDirections & MASK_NORTH) == MASK_NORTH;
        }

        /// <summary>
        /// If this command should allow trying to build to the east.
        /// </summary>
        /// <returns></returns>
        public bool shouldTryToBuildEast()
        {
            return (repetitionsAndDirections & MASK_EAST) == MASK_EAST;
        }
        
        /// <summary>
        /// If this command should allow trying to build to the south.
        /// </summary>
        /// <returns></returns>
        public bool shouldTryToBuildSouth()
        {
            return (repetitionsAndDirections & MASK_SOUTH) == MASK_SOUTH;
        }

        public bool shouldTryToBuildWest()
        {
            return (repetitionsAndDirections & MASK_WEST) == MASK_WEST;
        }

        public int roomsInSinglePass()
        {
            return roomsInSinglePassAsString().Length;
        }

        public string roomsInSinglePassAsString()
        {
            string toReturn = "";
            if (shouldTryToBuildNorth())
                toReturn += 'N';
            if (shouldTryToBuildEast())
                toReturn += 'E';
            if (shouldTryToBuildSouth())
                toReturn += 'S';
            if (shouldTryToBuildWest())
                toReturn += 'W';

            return toReturn;
        }

        public void decrementRoomsLeft()
        {
            if (!roomsDepleted)
                if (RoomsLeft > 0)
                    RoomsLeft--;
                else
                    roomsDepleted = true;
        }

        public void resetRoomsLeft()
        {
            RoomsLeft = (byte)(repetitionsAndDirections & MASK_REPETITIONS);
            roomsDepleted = false;
        }

        private byte[] toByteArray(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public double getWallChance()
        {
            return wallChance / 512.0;
        }

        internal static GenerationCommand[] parseArray(uint[] commands)
        {
            GenerationCommand[] toReturn = new GenerationCommand[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                toReturn[i] = new GenerationCommand(commands[i]);
            }
            return toReturn;
        }
    }
}
