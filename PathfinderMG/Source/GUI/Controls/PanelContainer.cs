using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace PathfinderMG.Core.Source.GUI.Controls
{
    /// <summary>
    /// Represents a vertical control container
    /// </summary>
    class PanelContainer : Control
    {
        #region Fields

        private const int HEADER_PADDING = 5;
        
        private List<Control> controls;
        private bool isBeingDragged;
        private Vector2 movement;

        #endregion

        #region Properties

        public int ControlMargin { get; set; }
        public bool Moveable { get; set; }
        public Color BackgroundColor { get; set; }
        public Vector2 Padding { get; set; }
        public List<Control> Controls { get { return controls; } }
        public Vector2 HeaderTitleDimensions
        {
            get
            {
                return font.MeasureString(Text);
            }
        }
        public Vector2 HeaderDimensions
        {
            get
            {
                return new Vector2(Dimensions.X, 2 * HEADER_PADDING + font.MeasureString(Text).Y);
            }
        }
        public Rectangle HeaderRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)HeaderDimensions.X, (int)HeaderDimensions.Y);
            }
        }
        public new Vector2 Dimensions
        {
            get
            {
                // Calculate width
                int longestControl = 0;
                int titleWidth = (int)HeaderTitleDimensions.X;
                if (controls.Count != 0)
                    longestControl = controls.Max(c => (int)c.Dimensions.X);

                int width = 2 * (int)Padding.X + ((longestControl > titleWidth) ? longestControl : titleWidth);

                // Calculate height
                int controlsHeightSum = 0;
                if (controls.Count != 0)
                    controlsHeightSum = controls.Sum(c => c.IsVisible ? (int)c.Dimensions.Y : 0);
            
                int height = (2 * HEADER_PADDING) + (int)HeaderTitleDimensions.Y + (2 * (int)Padding.Y) + controlsHeightSum + (ControlMargin * (controls.Count - 1));

                return new Vector2(width, height);
            }
        }
        public new Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Dimensions.X, (int)Dimensions.Y);
            }
        }

        #endregion

        #region Constructors

        public PanelContainer(Vector2 position, string title, Color fontColor, Color bgColor)
        {
            Position = position;
            Text = title;
            TextColor = fontColor;
            BackgroundColor = bgColor;

            texture = GameRoot.ContentMgr.Load<Texture2D>("Controls/PanelBackground");
            font = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");

            controls = new List<Control>();
        }

        #endregion

        #region Methods

        public void Add(Control control)
        {
            controls.Add(control);
            RepositionControls();
        }

        private void RepositionControls()
        {
            // Buttons should always be centered, other components should be aligned to the left
            for (int i = 0; i < controls.Count; i++)
            {
                if (!controls[i].IsVisible)
                    continue;
                if (i == 0)
                {
                    if (controls[i] is Button)
                        controls[i].Position = new Vector2(Position.X + (Dimensions.X - controls[i].Dimensions.X) / 2, 
                                                           Position.Y + HeaderDimensions.Y + Padding.Y + controls[i].MarginTop);
                    else
                        controls[i].Position = new Vector2(Position.X + Padding.X, 
                                                           Position.Y + HeaderDimensions.Y + Padding.Y + controls[i].MarginTop);
                }
                else
                {
                    if (controls[i] is Button)
                        controls[i].Position = new Vector2(Position.X + (Dimensions.X - controls[i].Dimensions.X) / 2, 
                                                           controls[i - 1].Position.Y + controls[i - 1].Rectangle.Height + ControlMargin + controls[i].MarginTop);
                    else
                        controls[i].Position = new Vector2(Position.X + Padding.X, 
                                                           controls[i-1].Position.Y + controls[i-1].Rectangle.Height + ControlMargin + controls[i].MarginTop);
                }
            }
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            RepositionControls();

            if (GameRoot.Mouse.Hovers(HeaderRectangle) || isBeingDragged)
            {
                movement = GameRoot.Mouse.GetDragMovement();
                isBeingDragged = GameRoot.Mouse.LeftButtonHeld() 
                                 && movement.Length() != 0;

                if (isBeingDragged)
                {
                    Position += movement;
                    RepositionControls();
                }
            }

            foreach (var control in controls)
                control.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw panel
            spriteBatch.Draw(texture, Rectangle, null, BackgroundColor, 0.0f, Vector2.Zero, SpriteEffects.None, 0);

            // Draw panel's header
            Color darkerBgColor = new Color(BackgroundColor.ToVector3() * new Vector3(.25f, .25f, .25f));
            Rectangle headerRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)HeaderDimensions.X, (int)HeaderDimensions.Y);

            spriteBatch.Draw(texture, headerRectangle, null, darkerBgColor, 0.0f, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, Text, new Vector2(headerRectangle.X + 5, headerRectangle.Y + HEADER_PADDING), Color.White);

            foreach (var control in controls)
                if(control.IsVisible)
                    control.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
