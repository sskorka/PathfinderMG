using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PathfinderMG.Core.Source.GUI.Controls
{
    /// <summary>
    /// Dims the background and serves as a base for other dialogs
    /// </summary>
    abstract class DialogBase : Control
    {
        #region Fields

        private Texture2D backgroundTex;
        private Rectangle fullScreenArea;

        protected List<Control> components;
        protected bool isCancellableThroughBackground;

        #endregion

        #region Properties

        public Color BackgroundColor { get; set; } = Color.Black;
        public bool Moveable { get; set; }
        public event EventHandler BackgroundClick;

        #endregion

        #region Constructors

        public DialogBase(bool isCancellableThroughBackground)
        {
            backgroundTex = GameRoot.ContentMgr.Load<Texture2D>("Controls/PanelBackground");
            fullScreenArea = new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT);
            this.isCancellableThroughBackground = isCancellableThroughBackground;
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            // Ignore all mouse events outside the dialog area
            if (!GameRoot.Mouse.Hovers(Rectangle) && GameRoot.Mouse.LeftButtonClicked())
            {
                if (isCancellableThroughBackground)
                    BackgroundClick?.Invoke(this, null);
                else
                    return;
            }    

            foreach (var c in components)            
                c.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTex, fullScreenArea, BackgroundColor);

            // Components need to be drawn from inheriting classes to
            // maintain the layer order. The spriteBatch.Draw()'s constructor
            // that takes a layer parameter might help -- look into that.
        }

        #endregion
    }
}
