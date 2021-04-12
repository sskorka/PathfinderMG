using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PathfinderMG.Core.Source.ScenarioCore.Pathfinders
{
    class AStarPathfinder : IPathfinder
    {
        #region Fields

        private const int diagonalCost = 14;
        private const int sidewayCost = 10;

        private Grid grid;
        private List<Node> neighbouringNodes;
        private int visualizationTime;

        #endregion

        #region Properties

        public readonly int MIN_VISUALIZATION_TIME = 5;
        public readonly int MAX_VISUALIZATION_TIME = 100;
        public readonly int DEFAULT_VISUALIZATION_TIME = 80;

        public static bool AllowDiagonalMovement { get; set; }
        public static bool InstantPathing { get; set; }

        public event EventHandler PathfindingFinished;

        public List<Node> OpenNodes { get; private set; }
        public List<Node> ClosedNodes { get; private set; }
        public Node CurrentNode { get; set; }
        public int VisualizationTime
        {
            get
            {
                return InstantPathing ? 0 : visualizationTime;
            }
            set
            {
                visualizationTime = (MAX_VISUALIZATION_TIME - value) + MIN_VISUALIZATION_TIME;
            }
        }

        #endregion

        #region Constructors

        public AStarPathfinder(Grid grid)
        {
            this.grid = grid;
            ClearPath();
            VisualizationTime = DEFAULT_VISUALIZATION_TIME;
        }

        #endregion

        #region Methods

        public async Task FindPathAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() =>
            {
                if (grid.Algorithm == null)
                    throw new NullReferenceException("Scenario grid does not have an assigned pathfinding algorithim!");

                while (true)
                {
                    if (OpenNodes.Count == 1 && OpenNodes[0] == grid.StartingNode)
                        CurrentNode = OpenNodes[0];
                    else
                        CurrentNode = GetNodeWithLowestFCost(OpenNodes);
                    Thread.Sleep(VisualizationTime);
                    OpenNodes.Remove(CurrentNode);
                    ClosedNodes.Add(CurrentNode);
                    Thread.Sleep(VisualizationTime);

                    if (CurrentNode == grid.TargetNode)
                    {
                        SetSolution(cancellationToken);
                        PathfindingFinished?.Invoke(this, null);
                        return;
                    }

                    neighbouringNodes = GetNeighbouringNodes(CurrentNode, allowDiagonals: AllowDiagonalMovement);
                    foreach (Node n in neighbouringNodes)
                    {
                        if(cancellationToken.IsCancellationRequested)
                        {
                            ClearPath();
                            PathfindingFinished?.Invoke(this, null);
                            return;
                        }

                        if (!n.IsTraversable || ClosedNodes.Contains(n))
                            continue;

                        if (IsNewPathShorter(CurrentNode, n) || !OpenNodes.Contains(n))
                        {
                            n.GCost = CurrentNode.GCost + GetDistance(CurrentNode, n);
                            n.HCost = GetDistance(n, grid.TargetNode);
                            n.Parent = CurrentNode;

                            if (!OpenNodes.Contains(n))
                                OpenNodes.Add(n);
                        }

                        Thread.Sleep(VisualizationTime / 3);
                    }
                }
            });     
        }

        public void ClearPath()
        {
            foreach (var node in grid.Nodes)
                node.Reset();

            OpenNodes = new List<Node>()
            {
                grid.StartingNode
            };
            ClosedNodes = new List<Node>();
            neighbouringNodes = new List<Node>();
        }

        /// <summary>
        /// Walk backwards from TargetNode and set parents' IsPartOfTheSolution.
        /// </summary>
        private void SetSolution(CancellationToken cancellationToken)
        {
            Node node = grid.TargetNode;
            List<Node> solution = new List<Node>();

            while (node != grid.StartingNode)
            {
                solution.Add(node);
                node = node.Parent;
            }

            solution.Reverse();
            foreach (var n in solution)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    ClearPath();
                    PathfindingFinished?.Invoke(this, null);
                    return;
                }

                n.IsPartOfTheSolution = true;
                //n = n.Parent;
                Thread.Sleep(VisualizationTime * 2);
            }
        }

        /// <summary>
        /// Calculates shortest path between two nodes, based on set constants
        /// </summary>
        private int GetDistance(Node from, Node to)
        {
            int distanceOfX = Math.Abs((int)from.Position.X - (int)to.Position.X);
            int distanceOfY = Math.Abs((int)from.Position.Y - (int)to.Position.Y);
            int output;

            if (distanceOfX < distanceOfY)
                output = (diagonalCost * distanceOfX) + (sidewayCost * (distanceOfY - distanceOfX));
            else
                output = (diagonalCost * distanceOfY) + (sidewayCost * (distanceOfX - distanceOfY));

            return output;
        }

        private Node GetNodeWithLowestFCost(List<Node> nodes)
        {
            Node lowestFCostNode = nodes[0];

            foreach (Node n in nodes)
                if (n.FCost < lowestFCostNode.FCost)
                    lowestFCostNode = n;

            return lowestFCostNode;
        }

        private List<Node> GetNeighbouringNodes(Node n, bool allowDiagonals)
        {
            List<Node> output = new List<Node>();
            int xTest, yTest;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    xTest = (int)n.Position.X + i;
                    yTest = (int)n.Position.Y + j;

                    // Check if node would be outside grid bounds
                    if (xTest < 0 || xTest >= (int)grid.NodeCount.X
                     || yTest < 0 || yTest >= (int)grid.NodeCount.Y)
                        continue;

                    // Skip over itself
                    if (i == 0 && j == 0)
                        continue;

                    // Skip over diagonals
                    if (Math.Abs(i * j) == 1 && !allowDiagonals)
                        continue;

                    output.Add(grid.Nodes[xTest, yTest]);
                }
            }

            return output;
        }

        private bool IsNewPathShorter(Node from, Node to)
        {
            bool output = (from.GCost + GetDistance(from, to)) < to.GCost;
            return output;
        }

        #endregion
    }
}
