using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PathfinderMG.Core.Source.ScenarioEditor
{
    /// <summary>
    /// A bottom-centered UI element that allows for fast item swapping.
    /// </summary>
    class Toolbar
    {
        // The toolbar should (for now) consist of [1] Wall tool, [2, 3] Start and target node, [4] Clear tool
        private const int NUMBER_OF_ITEMS = 4;

        private Texture2D toolbarTexture;
        private List<ToolbarItem> toolbarItems;
        private List<Texture2D> toolbarItemsTextures = new List<Texture2D>();       

        public Vector2 ItemDimensions { get; set; } = new Vector2(50,50);
        public Vector2 ItemMargin { get; set; } = new Vector2(5, 0);
        public int BottomMargin { get; set; } = 20;

        public Rectangle Area
        {
            get
            {
                Vector2 dimensions = new Vector2((NUMBER_OF_ITEMS * ItemDimensions.X) + (NUMBER_OF_ITEMS - 1) * ItemMargin.X, ItemDimensions.Y);
                return new Rectangle((Constants.SCREEN_WIDTH / 2) - ((int)dimensions.X / 2), 
                                     Constants.SCREEN_HEIGHT - (int)ItemDimensions.Y - BottomMargin, 
                                     (int)dimensions.X, (int)dimensions.Y);
            }
        }

        public Toolbar()
        {
            LoadContent();

            toolbarItems = new List<ToolbarItem>();
            for (int i = 0; i < NUMBER_OF_ITEMS; i++)
            {
                ToolbarItem item = new ToolbarItem()
                {
                    Texture = toolbarItemsTextures[i],
                    Area = new Rectangle(Area.X + (i * (int)ItemDimensions.X) + (i * (int)ItemMargin.X), 
                                         Area.Y, (int)ItemDimensions.X, (int)ItemDimensions.Y)
                };
                toolbarItems.Add(item);
            }
        }

        private void LoadContent()
        {
            toolbarTexture = GameRoot.ContentMgr.Load<Texture2D>("Editor/Toolbar");
            toolbarItemsTextures.Add(GameRoot.ContentMgr.Load<Texture2D>("Grid/ImpassableNode"));
            toolbarItemsTextures.Add(GameRoot.ContentMgr.Load<Texture2D>("Grid/StartNode"));
            toolbarItemsTextures.Add(GameRoot.ContentMgr.Load<Texture2D>("Grid/TargetNode"));
            toolbarItemsTextures.Add(GameRoot.ContentMgr.Load<Texture2D>("Editor/Rubber"));
        }

        public void Update(GameTime gameTime)
        {
            foreach (var item in toolbarItems)
                item.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw toolbar
            spriteBatch.Draw(toolbarTexture, Area, Color.White);

            // Draw toolbar items
            foreach (var item in toolbarItems)
                item.Draw(gameTime, spriteBatch);
        }
    }
}
