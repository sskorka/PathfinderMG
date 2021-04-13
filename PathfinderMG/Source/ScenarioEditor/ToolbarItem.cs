using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfinderMG.Core.Source.ScenarioEditor
{
    class ToolbarItem
    {
        public Rectangle Area { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Origin
        {
            get
            {
                return new Vector2(Area.Width / 2, Area.Height / 2);
            }
        }

        public ToolbarItem()
        {
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Area, null, Color.White, 0.0f, Origin, SpriteEffects.None, 0);            
        }
    }
}
