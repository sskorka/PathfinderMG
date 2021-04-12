using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfinderMG.Core.Source.ScenarioEditor
{
    class ToolbarItem
    {
        public Rectangle Area { get; set; }
        public Texture2D Texture { get; set; }

        public ToolbarItem()
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Area, Color.White);
        }
    }
}
