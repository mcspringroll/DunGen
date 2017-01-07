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
        /// Specifies whether this search should be
        /// primarily depth first or breadth first.
        /// </summary>
        public bool IsDepthFirst{get; set;}

        /// <summary>
        /// 
        /// </summary>
        private Node FrontNode { get; set; }

        private Node EndNode { get; set; }

        public int NumberOfRooms { get; set; }

        public bool HasNextRoom
        {
            get
            {
                return EndNode != null;
            }
        }

        public bool DoesNotHaveNextRoom
        {
            get
            {
                return EndNode == null;
            }
        }

        public FloorSprawler()
        {
            FrontNode = EndNode = null;
            IsDepthFirst = true;
            NumberOfRooms = 0;
        }

        public Room getNextRoom()
        {
            if (DoesNotHaveNextRoom)
                return null;
            if (IsDepthFirst)
                return EndNode.Data;
            else
                return FrontNode.Data;
        }

        public Room removeNextRoom()
        {
            Room temp;
            if (DoesNotHaveNextRoom)
                return null;
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

        public void addRoom(Room toAdd)
        {
            Node temp = new Node();
            temp.Data = toAdd;
            NumberOfRooms++;
            if(EndNode == null)
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

            while(this.FrontNode != null)
            {
                int i = 0;
                this.IsDepthFirst = ithBitEqualsOne(decisionByte, i++);
                newSprawler.IsDepthFirst = ithBitEqualsOne(decisionByte, i++); //this line is useless, addRoom() does not use IsDepthFirst
                newSprawler.addRoom(this.removeNextRoom());
                decisionByte = randomByte();
            }
            this.FrontNode = newSprawler.FrontNode;
            this.EndNode = newSprawler.EndNode;
            this.IsDepthFirst = originalDepthFirstValue;
        }

        private Byte randomByte()
        {
            Byte[] randVals = new Byte[1];
            (new Random()).NextBytes(randVals);
            return randVals[0];
        }

        private bool ithBitEqualsOne(Byte decisionByte, int i)
        {
            return ((decisionByte >> i) & 0x01) == 0x01; //had i++ here (instead of just i), probably wouldn't have hurt anything, but didn't seem too necessary?
        }

        private class Node
        {
            public Node Next { get; set; }
            public Node Previous { get; set; }
            public Room Data { get; set; }
        }
    }
}
