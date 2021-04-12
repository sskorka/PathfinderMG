using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PathfinderMG.Core.Source.GUI.Controls
{
    class Checkbox : Control
    {
        #region Fields

        protected const int DEFAULT_SIZE = 15;
        protected const int TEXT_MARGIN = 5;

        protected Texture2D uncheckedTexture, uncheckedHoveredTexture, checkedTexture, checkedHoveredTexture;
        protected bool isChecked;

        #endregion

        #region Properties

        public event EventHandler Click;

        public bool IsChecked { get { return isChecked; } set { isChecked = value; } }
        public Vector2 BoxDimensions { get; }
        public new Vector2 Dimensions
        {
            get
            {
                Vector2 fullDimensions = new Vector2(BoxDimensions.X + TEXT_MARGIN + font.MeasureString(Text).X, BoxDimensions.Y);
                base.Dimensions = fullDimensions;
                return fullDimensions;
            }
        }

        #endregion

        #region Constructors

        public Checkbox(string text, Color color)
        {
            BoxDimensions = new Vector2(DEFAULT_SIZE);
            Text = text;
            TextColor = color;
            LoadContent();

            base.Dimensions = Dimensions;
        }

        public Checkbox(Vector2 dimensions, string text, Color color)
        {
            BoxDimensions = new Vector2(dimensions.X, dimensions.Y);
            Text = text;
            TextColor = color;
            LoadContent();

            base.Dimensions = Dimensions;
        }

        #endregion

        #region Methods

        public void LoadContent()
        {
            uncheckedTexture = GameRoot.ContentMgr.Load<Texture2D>("Controls/CheckboxUnchecked");
            checkedTexture = GameRoot.ContentMgr.Load<Texture2D>("Controls/CheckboxChecked");
            uncheckedHoveredTexture = GameRoot.ContentMgr.Load<Texture2D>("Controls/CheckboxUncheckedHovered");
            checkedHoveredTexture = GameRoot.ContentMgr.Load<Texture2D>("Controls/CheckboxCheckedHovered");
            font = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            isHovering = false;

            if(GameRoot.Mouse.Hovers(Rectangle))
            {
                isHovering = true;

                if (GameRoot.Mouse.LeftButtonClicked())
                {
                    isChecked = !isChecked;
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsEnabled)
                return;

            Texture2D finalTex;

            if (isHovering && isChecked)
                finalTex = checkedHoveredTexture;
            else if (isHovering && !isChecked)
                finalTex = uncheckedHoveredTexture;
            else if (!isHovering && isChecked)
                finalTex = checkedTexture;
            else
                finalTex = uncheckedTexture;

            //Vector2 centeredTextPos = new Vector2(Position.X + base.Dimensions.X / 2, Position.Y + base.Dimensions.Y / 2);
            Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)BoxDimensions.X, (int)BoxDimensions.Y);
            Vector2 centeredTextPos = new Vector2(Position.X + BoxDimensions.X + TEXT_MARGIN, 
                                                  Position.Y + ((destinationRectangle.Height - font.MeasureString(Text).Y) / 2));

            spriteBatch.Draw(finalTex, destinationRectangle, null, Color.White, 0.0f, Vector2.Zero, new SpriteEffects(), 0);
            spriteBatch.DrawString(font, Text, centeredTextPos, TextColor);
        }

        #endregion
    }
}
