using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfinderMG.Core.Source.ScenarioEditor
{
    class ToolbarItem
    {
        public bool IsSelected { get; private set; } = false;
        public int Index { get; }
        public Rectangle Area { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D TextureSelected { get; private set; }
        public Vector2 Origin
        {
            get
            {
                return new Vector2(Area.Width / 2, Area.Height / 2);
            }
        }

        public ToolbarItem(int index)
        {
            Index = index;
            TextureSelected = GameRoot.ContentMgr.Load<Texture2D>("Editor/SelectedToolbarItem");
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Area, null, Color.White, 0.0f, Origin, SpriteEffects.None, 0);            

            if(IsSelected)
                spriteBatch.Draw(TextureSelected, Area, null, Color.White, 0.0f, Origin, SpriteEffects.None, 0);
        }
    }
}
