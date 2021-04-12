using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PathfinderMG.Core.Source.GUI.Controls
{
    class ButtonPack
    {
        public Texture2D TexDefault { get; set; }
        public Texture2D TexHovered { get; set; }
        public Texture2D TexSelected { get; set; }
    }

    class Button : Control
    {
        private bool isPressed;
        private ButtonPack buttonTextureSet;

        public const int DEFAULT_WIDTH = 160;
        public const int DEFAULT_HEIGHT = 40;

        public event EventHandler Click;

        #region Constructors

        public Button(bool isOriginAtCenter)
        {
            texture = GameRoot.ContentMgr.Load<Texture2D>("Controls/Button");
            font = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
            Dimensions = new Vector2(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            IsOriginAtCenter = isOriginAtCenter;
            TextColor = Color.Black;
        }

        public Button(bool isOriginAtCenter, ButtonPack textures)
        {
            buttonTextureSet = textures;
            font = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
            Dimensions = new Vector2(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            IsOriginAtCenter = isOriginAtCenter;
            TextColor = Color.Black;
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            if (!IsEnabled)
                return;

            isHovering = false;
            isPressed = false;

            if (GameRoot.Mouse.Hovers(Rectangle))
            {
                isHovering = true;
                isPressed = GameRoot.Mouse.LeftButtonPressed();

                if (GameRoot.Mouse.LeftButtonClicked())
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color color = Color.White;
            Color textColor = TextColor;
            Texture2D texFinal;

            if (buttonTextureSet != null)
                texFinal = buttonTextureSet.TexDefault;
            else
                texFinal = texture;                

            if (isHovering)
            {
                if (buttonTextureSet != null)
                    texFinal = buttonTextureSet.TexHovered;
                else
                    color = Color.Gray;
            }

            if (isPressed)
            {
                if (buttonTextureSet != null)
                    texFinal = buttonTextureSet.TexSelected;
                else
                    color = Color.DarkGray;
            }

            if (!IsEnabled)
            {
                color = Color.Black;
                textColor = Color.LightGray;
            }

            // Draw button
            spriteBatch.Draw(texFinal, Rectangle, color);

            // Draw string
            if (!string.IsNullOrEmpty(Text))
            {
                int applyOffset = IsOriginAtCenter ? 1 : 0;
                float x = Position.X + ((Rectangle.Width - font.MeasureString(Text).X) / 2) - (applyOffset * Rectangle.Width / 2);
                float y = Position.Y + ((Rectangle.Height - font.MeasureString(Text).Y) / 2) - (applyOffset * Rectangle.Height / 2);

                spriteBatch.DrawString(font, Text, new Vector2(x, y), textColor);
            }
        }

        #endregion
    }
}
