using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Xml.Linq;

namespace PathfinderMG.Core.Source.GUI.Controls
{
    class PaginatedListItem
    {
        #region Fields

        /// <summary>
        /// Might be a thousand items, no point in separately loading textures for each
        /// </summary>
        private static Texture2D itemTex = GameRoot.ContentMgr.Load<Texture2D>("Controls/PaginatedListItem");
        private static Texture2D itemHoveredTex = GameRoot.ContentMgr.Load<Texture2D>("Controls/PaginatedListItemHovered");
        private static Texture2D itemPressedTex = GameRoot.ContentMgr.Load<Texture2D>("Controls/PaginatedListItemPressed");
        private static Texture2D itemSelectedTex = GameRoot.ContentMgr.Load<Texture2D>("Controls/PaginatedListItemSelected");
        private static SpriteFont font = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");

        private const string byStr = "by ";

        #endregion

        #region Properties

        /// <summary>
        /// Set this to the width of ScenarioPaginatedList
        /// </summary>
        public static Rectangle ContainerArea { get; set; }
        public static int MarginHorizontal { get; set; } = 20;
        public static int MarginVertical { get; set; } = 65;

        public XDocument ScenarioFile { get; set; }
        public string ScenarioTitle { get; set; }
        public string ScenarioAuthor { get; set; }
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public bool IsSelected { get; set; }        
        public Vector2 Position { get; set; }
        public Rectangle Area
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, ContainerArea.Width - 2 * MarginHorizontal, 70);
            }
        }

        public event EventHandler ItemSelected;

        #endregion

        #region Constructors

        public PaginatedListItem(string title, string author)
        {
            ScenarioTitle = title;
            ScenarioAuthor = author;
        }

        #endregion

        #region Update/Draw

        public void Update(GameTime gameTime)
        {
            IsHovered = false;
            IsPressed = false;

            if (GameRoot.Mouse.Hovers(Area))
            {
                IsHovered = true;
                IsPressed = GameRoot.Mouse.LeftButtonPressed();

                if (GameRoot.Mouse.LeftButtonClicked())
                {
                    ItemSelected?.Invoke(this, new EventArgs());
                    IsSelected = true;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D finalTex;

            if (IsSelected)
                finalTex = itemSelectedTex;
            else if (IsPressed)
                finalTex = itemPressedTex;
            else if (IsHovered)
                finalTex = itemHoveredTex;
            else
                finalTex = itemTex;

            // Draw sprite
            spriteBatch.Draw(finalTex, Area, Color.White);

            // Draw text
            Vector2 scenarioTitleSize = font.MeasureString(ScenarioTitle);
            Vector2 bySize = font.MeasureString("by ");
            spriteBatch.DrawString(font, ScenarioTitle, new Vector2(Area.X + 10, Area.Y + 10), Color.White);
            spriteBatch.DrawString(font, byStr, new Vector2(Area.X + 10, Area.Y + scenarioTitleSize.Y + 10 + 3), Color.DarkGoldenrod);
            spriteBatch.DrawString(font, ScenarioAuthor, new Vector2(Area.X + 10 + bySize.X, Area.Y + scenarioTitleSize.Y + 10 + 3), Color.Goldenrod);
        }

        #endregion
    }
}
