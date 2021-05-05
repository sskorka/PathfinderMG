using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfinderMG.Core.Source.ScenarioCore.Pathfinders;
using PathfinderMG.Core.Source.ScenarioEditor;
using System;

namespace PathfinderMG.Core.Source.ScenarioCore
{
    class Grid
    {
        #region Fields

        private readonly Vector2 PREVIEW_MARGINS = new Vector2(20, 5);
        private readonly Vector2 DEFAULT_MARGINS = new Vector2(200, 30);

        private Vector2 gridOrigin, nodeCount, hoveredNode, screenSizeTarget;
        private float nodeSize;
        private bool isPreviewMode;
        private Texture2D nodeTex, startNodeTex, targetNodeTex, impassableNodeTex, hoveredNodeTex, pathNodeTex, openNode, closedNode;
        private Node[,] nodes;

        #endregion

        #region Properties

        public Vector2 GridSize
        {
            get
            {
                if (nodes != null)
                    return new Vector2(nodes.GetLength(0) * nodeSize, nodes.GetLength(1) * nodeSize);
                else
                    return new Vector2(1, 1);
            }
        }
        public Node[,] Nodes
        {
            get
            {
                return nodes;
            }
        }
        public Vector2 NodeCount
        {
            get { return new Vector2(nodes.GetLength(0), nodes.GetLength(1)); }
        }
        public bool DrawOpenClosedNodes { get; set; } = true;
        public Node StartingNode { get; private set; }
        public Node TargetNode { get; private set; }
        public IPathfinder Algorithm { get; private set; }

        public string Title { get; private set; }
        public string Author { get; private set; }
        public DateTime DateCreated { get; private set; }

        /// <summary>
        /// Fires when mouse cursor hovers over node
        /// </summary>
        public event EventHandler<Node> NodeHovered;

        /// <summary>
        /// Fires when mouse cursor leaves the node area
        /// </summary>
        public event EventHandler NodeLeft;

        #endregion

        #region Constructor/Load Content

        /// <summary>
        /// Represents both logical and physical grid.
        /// </summary>
        /// <param name="previewData">Information whether this grid should be a preview grid (boolean) and if so, provides appropriate dimensions for scaling</param>
        /// <param name="nodeSize">Target physical size of a node</param>
        /// <param name="scenario">Data from which to load the grid</param>
        public Grid(Tuple<bool, Rectangle> previewData, float nodeSize, ScenarioWrapper scenario)
        {
            LoadContent();
            this.nodeSize = nodeSize;
            hoveredNode = new Vector2(-1, -1);

            Title = scenario.Title;
            Author = scenario.Author;
            DateCreated = scenario.DateCreated;
            nodes = GetGridFromWrapper(scenario.Data);

            isPreviewMode = previewData.Item1;

            // Adjust the target dimensions of a screen, depending on whether it's a preview grid or not
            screenSizeTarget = previewData.Item1 ? new Vector2(previewData.Item2.Width, previewData.Item2.Height)
                                                 : new Vector2(Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT);

            ScaleContent(isPreview: previewData.Item1);

            // Set origin in a way that the grid is centered on-screen.
            // If it's a preview, add preview offset from previewData.
            int originX = ((int)screenSizeTarget.X / 2) - ((int)GridSize.X / 2) + (previewData.Item1 ? previewData.Item2.X : 0);
            int originY = ((int)screenSizeTarget.Y / 2) - ((int)GridSize.Y / 2) + (previewData.Item1 ? previewData.Item2.Y : 0);
            gridOrigin = new Vector2(originX, originY);
        }

        public void SetHoveredNodeType(Tool type)
        {
            switch (type)
            {
                case Tool.Rubber:
                    hoveredNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/HoveredNode");
                    break;
                case Tool.Wall:
                    hoveredNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/ImpassableNode");
                    break;
                case Tool.StartNode:
                    hoveredNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/StartNode");
                    break;
                case Tool.TargetNode:
                    hoveredNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/TargetNode");
                    break;
            }
        }

        private void LoadContent()
        {
            nodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/Node");
            startNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/StartNode");
            targetNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/TargetNode");
            impassableNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/ImpassableNode");
            hoveredNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/HoveredNode");
            pathNodeTex = GameRoot.ContentMgr.Load<Texture2D>("Grid/PathNode");
            openNode = GameRoot.ContentMgr.Load<Texture2D>("Grid/OpenNode");
            closedNode = GameRoot.ContentMgr.Load<Texture2D>("Grid/ClosedNode");
        }

        private void ScaleContent(bool isPreview)
        {
            // Apply margins
            Vector2 gridMaxViewport = screenSizeTarget - ((isPreview) ? PREVIEW_MARGINS : DEFAULT_MARGINS);

            // Check if default-sized nodes fall within gridViewport bounds
            // If they do, leave as it is

            Vector2 difference = gridMaxViewport - GridSize;
            if (difference.X >= 0 && difference.Y >= 0)
                return;

            // If the grid is too big, change nodeSize appropriately,
            // so that it stays within gridViewport bounds

            // Scale towards the bigger dimension; irrelevant with square grids

            float hFactor = gridMaxViewport.X / GridSize.X;
            float vFactor = gridMaxViewport.Y / GridSize.Y;
            float scaleFactor = hFactor < vFactor ? hFactor : vFactor;
            
            nodeSize *= scaleFactor;
        }
        
        #endregion

        #region Update/Draw

        public void Update(GameTime gameTime)
        {
            // No hover logic in preview mode
            if (isPreviewMode)
                return;

            hoveredNode = GetNodeCoordsFromLocation(GameRoot.Mouse.CurrentPosition);

            // Fire the event
            if (hoveredNode != new Vector2(-1, -1))
                NodeHovered?.Invoke(this, (Node)nodes[(int)hoveredNode.X, (int)hoveredNode.Y].Clone());
            else
                NodeLeft?.Invoke(this, null);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D finalTex;
            Rectangle destinationRect;

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    if (StartingNode.Position.X == i && StartingNode.Position.Y == j)
                        finalTex = startNodeTex;
                    else if (TargetNode.Position.X == i && TargetNode.Position.Y == j)
                        finalTex = targetNodeTex;
                    else if (!nodes[i, j].IsTraversable)
                        finalTex = impassableNodeTex;
                    else
                        finalTex = nodeTex;

                    if(Algorithm != null)
                        DrawAlgorithmicMagic(i, j, ref finalTex);

                    if (hoveredNode.X == i && hoveredNode.Y == j)
                        finalTex = hoveredNodeTex;

                    destinationRect = new Rectangle((int)gridOrigin.X + (i * (int)nodeSize), (int)gridOrigin.Y + (j * (int)nodeSize), (int)nodeSize - 2, (int)nodeSize - 2);
                    spriteBatch.Draw(finalTex, destinationRect, null, Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }
        }

        private void DrawAlgorithmicMagic(int x, int y, ref Texture2D tex)
        {
            if (nodes[x, y] == StartingNode || nodes[x, y] == TargetNode || !nodes[x, y].IsTraversable)
                return;
            if ((Algorithm as AStarPathfinder).OpenNodes.Contains(nodes[x, y]) && DrawOpenClosedNodes)
                tex = openNode;
            if ((Algorithm as AStarPathfinder).ClosedNodes.Contains(nodes[x, y]) && DrawOpenClosedNodes)
                tex = closedNode;
            if (nodes[x,y].IsPartOfTheSolution)
                tex = pathNodeTex;
        }

        #endregion

        #region Methods

        public void AssignNewAlgorithm(IPathfinder algorithm)
        {
            Algorithm = algorithm;
        }

        private Vector2 GetNodeCoordsFromLocation(Vector2 location)
        {
            // Check if location is inside the grid
            if (location.X < gridOrigin.X || location.X > gridOrigin.X + nodeSize * nodeCount.X
             || location.Y < gridOrigin.Y || location.Y > gridOrigin.Y + nodeSize * nodeCount.Y)
                return new Vector2(-1, -1);
            
            Vector2 adjustedLoc = location - gridOrigin;
            Vector2 output = new Vector2(Math.Min(Math.Max(0, (int)(adjustedLoc.X / nodeSize)), nodes.GetLength(0) - 1), 
                                         Math.Min(Math.Max(0, (int)(adjustedLoc.Y / nodeSize)), nodes.GetLength(1) - 1));
            return output;
        }

        private Node[,] GetGridFromWrapper(System.Collections.Generic.List<string> scenarioData)
        {
            nodeCount = new Vector2(scenarioData[0].Length, scenarioData.Count);
            Node[,] output = new Node[(int)nodeCount.X, (int)nodeCount.Y];

            for (int i = 0; i < nodeCount.X; i++)
                for (int j = 0; j < nodeCount.Y; j++)
                {
                    var nodeType = (Constants.NodeType)scenarioData[j][i];
                    output[i, j] = GetNewNode(nodeType, i, j);
                }            

            if (StartingNode == null || TargetNode == null)
                throw new Exception("Scenario does not include starting and/or target positions.");

            return output;
        }

        private Node GetNewNode(Constants.NodeType type, int x, int y)
        {
            switch (type)
            {
                // Empty space
                case Constants.NodeType.EmptyNode:
                    return new Node(isTraversable: true, new Vector2(x, y));

                // Target node
                case Constants.NodeType.TargetNode:
                    TargetNode = new Node(isTraversable: true, new Vector2(x, y));
                    return TargetNode;

                // Starting node
                case Constants.NodeType.StartNode:
                    StartingNode = new Node(isTraversable: true, new Vector2(x, y));
                    return StartingNode;

                // Impassable node
                case Constants.NodeType.WallNode:
                    return new Node(isTraversable: false, new Vector2(x, y));

                default:
                    throw new Exception($"Unknown node type \"{ type }\"");
            }
        }
        
        #endregion
    }
}
