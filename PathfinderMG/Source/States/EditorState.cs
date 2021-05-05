using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_Textbox;
using PathfinderMG.Core.Source.GUI;
using PathfinderMG.Core.Source.GUI.Controls;
using PathfinderMG.Core.Source.ScenarioCore;
using PathfinderMG.Core.Source.ScenarioEditor;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderMG.Core.Source.States
{
    class EditorState : State
    {
        #region Fields

        private readonly Dictionary<string, string> LABEL_STRINGS = new Dictionary<string, string>()
        {
            { "loc", "Loc: " },
            { "gridSize", "Grid size" },
            { "width", "Width: " },
            { "height", "Height: " }
        };
        private readonly Dictionary<string, string> BUTTON_STRINGS = new Dictionary<string, string>()
        {
            { "save", "Save" },
            { "reset", "Reset" }
        };
        private readonly Dictionary<string, string> VARIOUS_STRINGS = new Dictionary<string, string>()
        {
            { "settings", "Settings" }
        };

        private const int DEFAULT_ROWS_COLS = 4;
        private const int MAX_GRID_DIMENSIONS = 100;
        private const int MIN_GRID_SIZE = 3; // Start node, end node and one empty space

        private Toolbar toolbar;
        private Grid grid;
        private PanelContainer settingsPanel;
        private TextBox widthTextBox, heightTextBox;
        private Tuple<bool, Rectangle> previewData = Tuple.Create(false, Rectangle.Empty);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for initializing EditorState
        /// </summary>
        /// <param name="grid">Set to null to use a clear, default grid</param>
        public EditorState(GameRoot game, GraphicsDevice graphicsDevice, Grid grid) 
            : base(game, graphicsDevice)
        {
            Texture2D buttonTexture = GameRoot.ContentMgr.Load<Texture2D>("Controls/Button");
            SpriteFont buttonFont = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");

            if (grid == null)            
                this.grid = new Grid(previewData, Constants.DEFAULT_NODE_SIZE, GetDefaultScenario());            
            else
                this.grid = grid;

            LoadUI();
            toolbar = new Toolbar();
            toolbar.ToolbarSelectionChanged += Toolbar_ToolbarSelectionChanged;
                        
            this.grid.SetHoveredNodeType(Tool.Wall);
            this.grid.NodeClicked += Grid_NodeClicked;
        }

        #endregion

        #region Methods

        private ScenarioWrapper GetDefaultScenario()
        {
            List<string> scenarioData = GetScenarioData(DEFAULT_ROWS_COLS, DEFAULT_ROWS_COLS, forDefaultScenario: true);

            return new ScenarioWrapper()
            {
                Title = "Untitled",
                Author = "No author",
                DateCreated = DateTime.Now,
                Data = scenarioData
            };
        }

        private List<string> GetScenarioData(int width, int height, bool forDefaultScenario = false)
        {
            List<string> scenarioData = new List<string>();
            StringBuilder sb = new StringBuilder();

            // Fill the list with empty spaces
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (forDefaultScenario)
                    {
                        // Place start node and target node in the corners
                        if (i == 0 && j == 0)
                        {
                            sb.Append((char)Constants.NodeType.StartNode);
                            continue;
                        }
                        else if (i == (DEFAULT_ROWS_COLS - 1) && j == (DEFAULT_ROWS_COLS - 1))
                        {
                            sb.Append((char)Constants.NodeType.TargetNode);
                            continue;
                        }
                    }
                    
                    sb.Append((char)Constants.NodeType.EmptyNode);
                }
                scenarioData.Add(sb.ToString());
                sb.Clear();
            }

            return scenarioData;
        }

        private void ResizeGrid(int width, int height)
        {
            // Let's try enlarging the grid first
            // Will try to preserve current grid setup

            if (width * height < MIN_GRID_SIZE)
                throw new Exception("Requested size is too small!");

            List<string> newMap = GetScenarioData(width, height);

            Vector2 startPosition = grid.StartingNode.Position;
            Vector2 targetPosition = grid.TargetNode.Position;

            /**
             * Insert start node
             * 
             * If the previous position is in bounds with the new map, place it back.
             * Otherwise place it at the top-left corner.
             */
            if((int)startPosition.X < width && (int)startPosition.Y < height)
                newMap[(int)startPosition.X] = newMap[(int)startPosition.X]
                                                    .Remove((int)startPosition.Y, 1)
                                                    .Insert((int)startPosition.Y, ((char)Constants.NodeType.StartNode).ToString());
            else
                newMap[0] = newMap[0]
                            .Remove(0, 1)
                            .Insert(0, ((char)Constants.NodeType.StartNode).ToString());

            /**
             * Insert target node
             * 
             * Previous steps apply, but the target node should be placed under the start node.
             */
            if ((int)targetPosition.X < width && (int)targetPosition.Y < height)
                newMap[(int)targetPosition.X] = newMap[(int)targetPosition.X]
                                                     .Remove((int)targetPosition.Y, 1)
                                                     .Insert((int)targetPosition.Y, ((char)Constants.NodeType.TargetNode).ToString());
            else
                newMap[0] = newMap[0]
                            .Remove(1, 1)
                            .Insert(1, ((char)Constants.NodeType.TargetNode).ToString());

            var newScenario = new ScenarioWrapper()
            {
                Title = grid.Title,
                Author = grid.Author,
                DateCreated = grid.DateCreated,
                Data = newMap
            };

            grid = new Grid(previewData, Constants.DEFAULT_NODE_SIZE, newScenario);
            grid.NodeClicked += Grid_NodeClicked;
        }

        private Constants.NodeType GetNodeTypeFromToolType(Tool tool)
        {
            switch(tool)
            {
                case Tool.Wall:
                    return Constants.NodeType.WallNode;
                case Tool.StartNode:
                    return Constants.NodeType.StartNode;
                case Tool.TargetNode:
                    return Constants.NodeType.TargetNode;
                case Tool.Rubber:
                    return Constants.NodeType.EmptyNode;
                default:
                    throw new Exception($"Unknown tool type provided! ${ tool }");
            }
                
        }

        private void LoadUI()
        {
            SpriteFont font = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
            components = new List<Control>();

            Vector2 panelAnchor = new Vector2(100, 50);
            string panelTitle = VARIOUS_STRINGS["settings"];
            Color panelFontColor = Color.White;
            Color panelBgColor = new Color(20, 20, 20);

            settingsPanel = new PanelContainer(panelAnchor, panelTitle, panelFontColor, panelBgColor)
            {
                ControlMargin = 15,
                Padding = new Vector2(10, 10),
                Moveable = true
            };

            // The call order is important.
            // Remember that PanelContainer takes care of positioning :)
            ConstructLabel_SettingsPanel(LABEL_STRINGS["gridSize"]);
            ConstructLabel_SettingsPanel(LABEL_STRINGS["width"]);
            ConstructTextBox_SettingsPanel(font, ref widthTextBox);
            ConstructLabel_SettingsPanel(LABEL_STRINGS["height"]);
            ConstructTextBox_SettingsPanel(font, ref heightTextBox);
            ConstructLabel_SettingsPanel(LABEL_STRINGS["loc"]);
            ConstructButtons_SettingsPanel();

            components.Add(settingsPanel);
        }

        private void ConstructTextBox_SettingsPanel(SpriteFont font, ref TextBox tbInput)
        {
            Rectangle tbArea = new Rectangle(0, 0, (int)font.MeasureString("999").X * 3, 20);

            tbInput = new TextBox(tbArea, MAX_GRID_DIMENSIONS.ToString().Length,
                                                          DEFAULT_ROWS_COLS.ToString(), GameRoot.RootGraphicsDevice, font,
                                                          TextBox.InputType.NumbersOnly, Color.White, Color.Gray, Color.DimGray, 30)
            {
                Rectangle = new Rectangle(tbArea.X, tbArea.Y, tbArea.Width, tbArea.Height),
                MarginTop = -15,
                Active = false
            };
            
            tbInput.Renderer.Color = Color.White;
            tbInput.Cursor.Selection = new Color(Color.Purple, .4f);

            tbInput.EnterDown += TbInput_EnterDown;
            tbInput.InputChanged += TbInput_InputChanged;

            settingsPanel.Add(tbInput);
        }

        private void ConstructLabel_SettingsPanel(string str)
        {
            Label label;

            label = new Label(str);

            settingsPanel.Add(label);
        }

        private void ConstructButtons_SettingsPanel()
        {
            ButtonPack buttonPack = new ButtonPack()
            {
                TexDefault = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/Button"),
                TexHovered = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/ButtonHovered"),
                TexSelected = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/ButtonSelected")
            };

            Button saveScenario = new Button(isOriginAtCenter: false, buttonPack)
            {
                Text = BUTTON_STRINGS["save"]
            };

            saveScenario.Click += SaveScenario_Click;

            Button resetScenario = new Button(isOriginAtCenter: false, buttonPack)
            {
                Text = BUTTON_STRINGS["reset"]
            };

            resetScenario.Click += ResetScenario_Click;

            settingsPanel.Add(saveScenario);
            settingsPanel.Add(resetScenario);
        }

        #endregion

        #region Event Handlers

        private void TbInput_InputChanged(object sender, string e)
        {
            int inputValue;
            if (Int32.TryParse(e, out inputValue))
            {
                if (inputValue > MAX_GRID_DIMENSIONS)
                    inputValue = MAX_GRID_DIMENSIONS;
                                
                (sender as TextBox).Text.String = inputValue.ToString();
            }
        }

        private void TbInput_EnterDown(object sender, KeyboardInput.KeyEventArgs e)
        {
            int width = Int32.Parse(widthTextBox.Text.String);
            int height = Int32.Parse(heightTextBox.Text.String);

            if (width == (int)grid.GridSize.X && height == (int)grid.GridSize.Y)
                return;

            ResizeGrid(width, height);
        }

        private void ResetScenario_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SaveScenario_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EditorEntryDialog_BackgroundClick(object sender, EventArgs e)
        {
        }

        private void Toolbar_ToolbarSelectionChanged(object sender, Tool e)
        {
            grid.SetHoveredNodeType(e);
        }

        private void Grid_NodeClicked(object sender, Node e)
        {
            ScenarioWrapper currentState = grid.GetGridWrapper();
            currentState.Data[(int)e.Position.Y] = currentState.Data[(int)e.Position.Y]
                                                        .Remove((int)e.Position.X, 1)
                                                        .Insert((int)e.Position.X, ((char)GetNodeTypeFromToolType(toolbar.CurrentTool)).ToString());
            
            grid = new Grid(previewData, Constants.DEFAULT_NODE_SIZE, currentState);
            grid.NodeClicked += Grid_NodeClicked;
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            grid.Update(gameTime);
            toolbar.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            grid.Draw(gameTime, spriteBatch);
            toolbar.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        #endregion
    }
}
