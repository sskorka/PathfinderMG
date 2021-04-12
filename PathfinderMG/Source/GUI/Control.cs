using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PathfinderMG.Core.Source.GUI
{
    abstract class Control
    {
        #region Fields

        protected SpriteFont font;
        protected Texture2D texture;
        protected bool isHovering;
        protected int offset;

        private string text;

        #endregion

        #region Properties

        public event EventHandler TextChanged; 

        public string Text 
        {
            get
            {
                return text;
            }
            set 
            {
                text = value;
                TextChanged?.Invoke(this, null);
            }
        }
        public Color TextColor { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Dimensions { get; set; }
        public int MarginTop { get; set; }
        public bool IsOriginAtCenter { get; set; } = false;
        public bool IsEnabled { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public Rectangle Rectangle
        {
            get
            {
                int applyOffset = IsOriginAtCenter ? 1 : 0;
                return new Rectangle((int)Position.X - applyOffset * ((int)Dimensions.X / 2), 
                                     (int)Position.Y - applyOffset * ((int)Dimensions.Y / 2), 
                                     (int)Dimensions.X, (int)Dimensions.Y);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// This constructor exists for classes like DialogBase who do not
        /// need to call its base constructor, yet it's necessary for them
        /// to initialize
        /// </summary>
        protected Control() { }

        protected Control(Texture2D texture, SpriteFont font)
        {
            this.texture = texture;
            this.font = font;
        }
        
        protected Control(Texture2D texture, Vector2 position, Vector2 dimensions, SpriteFont font)
        {
            this.texture = texture;
            this.font = font;
            Position = position;
            Dimensions = dimensions;
        }

        #endregion

        #region Update/Draw

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        #endregion
    }
}
