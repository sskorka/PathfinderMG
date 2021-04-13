using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfinderMG.Core.Source.GUI.Controls;
using PathfinderMG.Core.Source.ScenarioCore;
using System;
using System.Collections.Generic;

namespace PathfinderMG.Core.Source.ScenarioLoader
{
    class PreviewWindow
    {
        private readonly Dictionary<string, string> PREVIEW_STRINGS = new Dictionary<string, string>()
        {
            { "load", "Load"},
            { "delete", "Delete" },
            { "previewInfo", "Preview will be displayed here"}
        };

        #region Properties

        public Grid PreviewedGrid { get; set; }
        public ScenarioWrapper Scenario { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Area { get; set; }
        public Color BackgroundColor { get; set; }

        // UI Elements
        public Button LoadButton { get; set; }
        public Button DeleteButton { get; set; }
        public Label PreviewLabel { get; set; }
        /// <summary>
        /// Margin from each side
        /// </summary>
        public int PreviewMargin { get; set; } = 20;
        /// <summary>
        /// Additional margin
        /// </summary>
        public int MarginBottom { get; set; } = 50;
        public int MarginSides { get; set; } = 5;

        public event EventHandler<ScenarioWrapper> LoadRequested;

        #endregion

        #region Constructors

        public PreviewWindow(Rectangle area, Color bgColor)
        {
            Area = area;
            BackgroundColor = bgColor;

            LoadUI();
        }

        #endregion

        #region Methods

        public void LoadPreview(ScenarioWrapper scenario)
        {
            Scenario = scenario;

            PreviewLabel.IsVisible = false;
            Tuple<bool, Rectangle> previewInfo = Tuple.Create(true, new Rectangle(Area.X + PreviewMargin, 
                                                                                  Area.Y + PreviewMargin,
                                                                                  Area.Width - PreviewMargin * 2, 
                                                                                  Area.Height - PreviewMargin * 2 - MarginBottom));
            PreviewedGrid = new Grid(previewInfo, Constants.PREVIEW_NODE_SIZE, scenario);
        }

        private void LoadUI()
        {
            Texture = GameRoot.ContentMgr.Load<Texture2D>("Controls/PanelBackground");
            SpriteFont font = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
            ButtonPack buttonPack = new ButtonPack()
            {
                TexDefault = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/Button"),
                TexHovered = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/ButtonHovered"),
                TexSelected = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/ButtonSelected")
            };

            LoadButton = new Button(isOriginAtCenter: true, buttonPack)
            {
                Text = PREVIEW_STRINGS["load"],
                Position = new Vector2(Area.X + (Area.Width / 2) - (Button.DEFAULT_WIDTH / 2) - MarginSides,
                           Area.Y + Area.Height - MarginBottom)
            };

            LoadButton.Click += LoadButton_Click;

            DeleteButton = new Button(isOriginAtCenter: true, buttonPack)
            {
                Text = PREVIEW_STRINGS["delete"],
                Position = new Vector2(Area.X + (Area.Width / 2) + (Button.DEFAULT_WIDTH / 2) + MarginSides,
                                       Area.Y + Area.Height - MarginBottom)
            };

            DeleteButton.Click += DeleteButton_Click;

            string labelStr = PREVIEW_STRINGS["previewInfo"];
            PreviewLabel = new Label(labelStr)
            {
                Position = new Vector2(Area.X + (Area.Width / 2) - (font.MeasureString(labelStr).X / 2),
                                       Area.Y + (Area.Height / 2) - (font.MeasureString(labelStr).Y / 2) - MarginBottom),
                TextColor = Color.Black
            };
        }

        #endregion

        #region Event Handlers

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Come back here when messagebox or dialog are ready
            // For now just open the explorer where scenarios are saved
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadRequested?.Invoke(this, Scenario);
        }

        #endregion

        #region Update/Draw

        public void Update(GameTime gameTime)
        {
            if(PreviewedGrid != null)
                PreviewedGrid.Update(gameTime);
            PreviewLabel.Update(gameTime);
            LoadButton.Update(gameTime);
            DeleteButton.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Area, BackgroundColor);

            if (PreviewedGrid != null)
                PreviewedGrid.Draw(gameTime, spriteBatch);
            PreviewLabel.Draw(gameTime, spriteBatch);
            LoadButton.Draw(gameTime, spriteBatch);
            DeleteButton.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}