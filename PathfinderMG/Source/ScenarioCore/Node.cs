using Microsoft.Xna.Framework;
using System;

namespace PathfinderMG.Core.Source.ScenarioCore
{
    class Node : ICloneable
    {
        public bool IsTraversable { get; private set; }
        public bool IsPartOfTheSolution { get; set; } = false;
        public bool IsInOpenList { get; set; }
        public bool IsInClosedList { get; set; }

        /// <summary>
        /// Node's position in GRID SPACE
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Distance from starting node
        /// </summary>
        public int GCost { get; set; }

        /// <summary>
        /// Distance from target node
        /// </summary>
        public int HCost { get; set; }

        /// <summary>
        /// Sum of gCost and hCost
        /// </summary>
        public int FCost => GCost + HCost;

        /// <summary>
        /// The preceeding node for tracing back the fastest path
        /// </summary>
        public Node Parent { get; set; }

        public Node(bool isTraversable, Vector2 position)
        {
            IsTraversable = isTraversable;
            Position = position;
        }

        public void Reset()
        {
            GCost = 0;
            HCost = 0;
            IsPartOfTheSolution = false;
            IsInOpenList = false;
            IsInClosedList = false;
            Parent = null;
        }

        public object Clone()
        {
            return new Node(IsTraversable, Position)
            {
                IsPartOfTheSolution = IsPartOfTheSolution,
                IsInOpenList = IsInOpenList,
                IsInClosedList = IsInClosedList,
                GCost = GCost,
                HCost = HCost
            };
        }
    }
}
