using System;
using DungeonAPI.Definitions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAPI.Generation
{
    /// <summary>
    /// Controls the construction of floor by
    /// keeping track of the newest and oldest
    /// rooms added.  Allows for depth or breadth
    /// first style generation
    /// </summary>
    class FloorSprawler
    {
        /// <summary>
        /// Generic constructor to initialize an
        /// object of this class.
        /// </summary>
        public FloorSprawler()
        {
            FrontNode = EndNode = null;
            IsDepthFirst = true;
            NumberOfRooms = 0;
        }

        /// <summary>
        /// Specifies whether this search should be
        /// primarily depth first or breadth first.
        /// </summary>
        public bool IsDepthFirst { get; set; }

        /// <summary>
        /// The "oldest" node in the sprawler.  If
        /// the sprawler is shuffled, it is still
        /// considered the "oldest" but should not
        /// be thought of as the chronologically
        /// oldest room added.
        /// </summary>
        private Node FrontNode { get; set; }

        /// <summary>
        /// The "newest" node in the sprawler.  If
        /// the sprawler is shuffled, it is still
        /// considered the "newest" but should not
        /// be thought of as the chronologically
        /// newest room added.
        /// 
        /// When a room is added, it always
        /// becomes the EndNode.
        /// </summary>
        private Node EndNode { get; set; }

        /// <summary>
        /// The number of rooms currently within
        /// the sprawler.
        /// </summary>
        public int NumberOfRooms { get; private set; }

        /// <summary>
        /// If there are any rooms left in the
        /// sprawler.
        /// </summary>
        public bool HasNextRoom
        {
            get
            {
                return EndNode != null;
            }
        }

        /// <summary>
        /// If there are no rooms left in the
        /// sprawler.
        /// </summary>
        public bool DoesNotHaveNextRoom
        {
            get
            {
                return EndNode == null;
            }
        }

        /// <summary>
        /// Retrieves the "next" room from the
        /// sprawler.  The end that is pulled
        /// from is specified by IsDepthFirst.
        /// </summary>
        /// <returns>
        /// The newest room if IsDepthFirst is
        /// true.  Otherwise, the oldest room.
        /// </returns>
        public Room getNextRoom()
        {
            if (DoesNotHaveNextRoom)
                return null;
            if (IsDepthFirst)
                return EndNode.Data;
            else
                return FrontNode.Data;
        }


        /// <summary>
        /// Removes the "next" room from the
        /// sprawler.  Also returns the room.
        /// The end that is pulled from is 
        /// specified by IsDepthFirst.
        /// </summary>
        /// <returns>
        /// The newest room if IsDepthFirst is
        /// true.  Otherwise, the oldest room.
        /// </returns>
        public Room removeNextRoom()
        {
            Room temp;
            if (DoesNotHaveNextRoom)
                return null;

            //There is only one node.
            if (EndNode == FrontNode)
            {
                temp = FrontNode.Data;
                EndNode = FrontNode = null;
            }
            else if (IsDepthFirst)
            {
                temp = EndNode.Data;
                EndNode.Previous.Next = null;
                EndNode = EndNode.Previous;
            }
            else
            {
                temp = FrontNode.Data;
                FrontNode.Next.Previous = null;
                FrontNode = FrontNode.Next;
            }
            NumberOfRooms--;
            return temp;
        }

        /// <summary>
        /// Addes a new room to the 
        /// </summary>
        /// <param name="toAdd"></param>
        public void addRoom(Room toAdd)
        {
            if (toAdd == null)
                return;

            Node temp = new Node();
            temp.Data = toAdd;
            NumberOfRooms++;
            if (EndNode == null)
            {
                FrontNode = EndNode = temp;
            }
            else
            {
                EndNode.Next = temp;
                temp.Previous = EndNode;
                EndNode = temp;
            }
        }

        public void RemoveLockedRooms()
        {
            Node currentRoomNode = FrontNode;
            while (currentRoomNode != null)
            {
                if (currentRoomNode.Data.HasAllNeighbors)
                {
                    Node previousNode = currentRoomNode.Previous;
                    Node nextNode = currentRoomNode.Next;

                    if (nextNode != null)
                    {
                        nextNode.Previous = previousNode;
                    }
                    else //is the end node
                    {
                        EndNode = previousNode;
                        if (EndNode == null || EndNode.Previous == null)
                        {
                            FrontNode = EndNode;
                        }
                    }
                    if (previousNode != null)
                    {
                        previousNode.Next = nextNode;
                    }
                    else
                    {
                        FrontNode = nextNode;
                        if (FrontNode == null || FrontNode.Next == null)
                        {
                            EndNode = FrontNode;
                        }
                    }
                    if (nextNode == null && previousNode == null)
                    {
                        FrontNode = EndNode = null;
                    }

                    NumberOfRooms--;
                }
                currentRoomNode = currentRoomNode.Next;
            }
        }


        /// <summary>
        /// Takes all rooms in this FloorSprawler and
        /// rebuilds this FloorSprawler's contents so
        /// that the order from "front" to "back" is
        /// different than before.
        /// 
        /// This is done by taking either the first or
        /// last element of the existing FloorSprawler
        /// and adding it to either the "front" or the
        /// "back" of a new FloorSprawler.  Should kind
        /// of turn the contents inside out.
        /// 
        /// I checked how this performs in an outside
        /// project.  It performs semi-well with a high 
        /// tendency to leave the outer sections 
        /// less sorted than inner sections.  Therefore
        /// it should be rewritten.
        /// </summary>
        public void shuffleRooms()
        {
            FloorSprawler newSprawler = new FloorSprawler();
            Byte decisionByte = randomByte();
            bool originalDepthFirstValue = this.IsDepthFirst;
            int i = 0;
            while (this.FrontNode != null)
            {
                /*Though there is method call overhead here and more visible code that
                * needs to run, using the ithBitEqualsOne method appears to be faster
                * on large sets than the simpler solution of:
                *     (new Random()).Next(2) == 0;
                * Having it iterate through decisionByte until it runs out of space
                * also appears to be significantly faster as well.
                */
                this.IsDepthFirst = ithBitEqualsOne(decisionByte, i++);
                
                newSprawler.addRoom(this.removeNextRoom());
                if (i == 8)//8 should probably be wrapped in a constant
                {
                    decisionByte = randomByte();
                    i = 0;
                }
            }
            this.FrontNode = newSprawler.FrontNode;
            this.EndNode = newSprawler.EndNode;
            this.NumberOfRooms = newSprawler.NumberOfRooms;
            this.IsDepthFirst = originalDepthFirstValue;
        }

        /// <summary>
        /// Gets a random byte.
        /// </summary>
        /// <returns></returns>
        private Byte randomByte()
        {
            Byte[] randVals = new Byte[1];
            (new Random()).NextBytes(randVals);
            return randVals[0];
        }

        /// <summary>
        /// Whether or not the ith bit in decisionByte
        /// is 1 or 0.
        /// </summary>
        /// <param name="decisionByte"></param>
        /// <param name="i"></param>
        /// <returns>True if bit i in decisionByte is on.</returns>
        private bool ithBitEqualsOne(Byte decisionByte, int i)
        {
            return ((decisionByte >> i) & 0x01) == 0x01; //had i++ here (instead of just i), probably wouldn't have hurt anything, but didn't seem too necessary?
        }

        /// <summary>
        /// Simple node container for a room.
        /// </summary>
        private class Node
        {
            public Node() { }

            /// <summary>
            /// The node "in front of" this node.
            /// </summary>
            public Node Next { get; set; }

            /// <summary>
            /// The node "behind" this node.
            /// </summary>
            public Node Previous { get; set; }

            /// <summary>
            /// The Room that is stored in this
            /// node.
            /// </summary>
            public Room Data { get; set; }
        }
    }
}
