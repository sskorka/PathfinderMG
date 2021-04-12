using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfinderMG.Core.Source.ScenarioEditor;
using System;

namespace PathfinderMG.Core.Source.States
{
    class EditorState : State
    {
        private Toolbar toolbar;

        public EditorState(GameRoot game, GraphicsDevice graphicsDevice) 
            : base(game, graphicsDevice)
        {
            Texture2D buttonTexture = GameRoot.ContentMgr.Load<Texture2D>("Controls/Button");
            SpriteFont buttonFont = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");

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
