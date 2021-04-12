using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfinderMG.Core.Source.GUI;
using System.Collections.Generic;

namespace PathfinderMG.Core.Source.States
{
    abstract class State
    {
        protected GraphicsDevice graphicsDevice;
        protected GameRoot game;

        protected List<Control> components;

        public State(GameRoot game, GraphicsDevice graphicsDevice)
        {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
        }

        public virtual void Update(GameTime gameTime)
        {
            if(components != null)
                foreach (var component in components)
                    component.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (components != null)
                foreach (var component in components)
                    component.Draw(gameTime, spriteBatch);
        }
    }
}
