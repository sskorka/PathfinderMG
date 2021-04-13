using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfinderMG.Core.Source.ScenarioCore;
using PathfinderMG.Core.Source.ScenarioEditor;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderMG.Core.Source.States
{
    class EditorState : State
    {
        private const int DEFAULT_ROWS_COLS = 4;

        private ScenarioWrapper defaultScenario
        {
            get
            {
                List<string> scenarioData = new List<string>();
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < DEFAULT_ROWS_COLS; i++)
                {
                    for (int j = 0; j < DEFAULT_ROWS_COLS; j++)
                        sb.Append(Constants.NODE_EMPTY);
                    scenarioData.Add(sb.ToString());
                    sb.Clear();
                }
                
                return new ScenarioWrapper()
                {
                    Title = "Untitled",
                    Author = "No author",
                    DateCreated = DateTime.Now,
                    Data = scenarioData
                };
            }
        }            

        private Toolbar toolbar;
        private Grid grid;

        /// <summary>
        /// Represents the scenario editor state
        /// </summary>
        /// <param name="grid">Provide grid for </param>
        public EditorState(GameRoot game, GraphicsDevice graphicsDevice, Grid grid) 
            : base(game, graphicsDevice)
        {
            Texture2D buttonTexture = GameRoot.ContentMgr.Load<Texture2D>("Controls/Button");
            SpriteFont buttonFont = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");


            if (grid == null)
            {
                Tuple<bool, Rectangle> previewData = Tuple.Create(false, Rectangle.Empty);
                grid = new Grid(previewData, Constants.DEFAULT_NODE_SIZE, defaultScenario);
            }
            else
                this.grid = grid;

            toolbar = new Toolbar();
        }

        private void EditorEntryDialog_BackgroundClick(object sender, EventArgs e)
        {
        }

        #region Methods

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            toolbar.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            base.Draw(gameTime, spriteBatch);
            toolbar.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        #endregion
    }
}
