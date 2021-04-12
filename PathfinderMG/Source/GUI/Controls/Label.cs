using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfinderMG.Core.Source.GUI.Controls
{
    class Label : Control
    {
        #region Constructors

        public Label(string text)
        {
            Text = text;
            font = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
            TextColor = Color.White;
            Dimensions = font.MeasureString(Text);
            TextChanged += Label_TextChanged;
        }

        #endregion

        #region Event Handlers

        private void Label_TextChanged(object sender, System.EventArgs e)
        {
            Dimensions = font.MeasureString(Text);
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(Text) && IsVisible)
            {
                spriteBatch.DrawString(font, Text, Position, TextColor, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }

        #endregion
    }
}
