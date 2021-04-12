using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfinderMG.Core.Source.GUI;
using PathfinderMG.Core.Source.GUI.Controls;
using PathfinderMG.Core.Source.ScenarioLoader;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PathfinderMG.Core.Source.States
{
    class LoadState : State
    {
        #region Fields

        private const string BACK_STRING = "Back";
        private readonly Vector2 BACK_BUTTON_SIZE = new Vector2(80, 40);

        private const int PREVIEW_WIDTH = 600;
        private const int SPL_WIDTH = 350;
        private const int PREVIEW_SPL_TOTAL_WIDTH = PREVIEW_WIDTH + SPL_WIDTH;
        private const int PREVIEW_SPL_HEIGHT = 600;

        private PreviewWindow preview;
        private ScenarioPaginatedList spl;
        private ScenarioLoader.ScenarioLoader loader;

        private bool editorMode;
        private Button returnButton;
        
        #endregion

        #region Constructors

        public LoadState(GameRoot game, GraphicsDevice graphicsDevice, bool editorMode) 
            : base(game, graphicsDevice)
        {
            loader = new ScenarioLoader.ScenarioLoader();
            this.editorMode = editorMode;
            LoadUI();

            var scenarios = loader.FetchAllScenarios();
            spl.LoadScenarios(scenarios);
        }

        #endregion

        #region Methods

        private void LoadUI()
        {
            preview = new PreviewWindow(new Rectangle((Constants.SCREEN_WIDTH - PREVIEW_SPL_TOTAL_WIDTH) / 2,
                                                     (Constants.SCREEN_HEIGHT - PREVIEW_SPL_HEIGHT) / 2,
                                                     PREVIEW_WIDTH, PREVIEW_SPL_HEIGHT), 
                                        Color.White);
            spl = new ScenarioPaginatedList(new Rectangle(preview.Area.X + preview.Area.Width, 
                                                          preview.Area.Y, 
                                                          SPL_WIDTH,
                                                          PREVIEW_SPL_HEIGHT), 
                                            Color.DimGray, loader);
            
            PaginatedListItem.ContainerArea = spl.Area;
            spl.ScenarioSelected += Spl_ScenarioSelected;
            preview.LoadRequested += Preview_LoadRequested;

            SpriteFont buttonFont = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
            ButtonPack buttonPack = new ButtonPack()
            {
                TexDefault = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BackButton"),
                TexHovered = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BackButtonHovered"),
                TexSelected = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BackButtonSelected")
            };

            // Load "Back to menu" button
            int returnButtonMargin = 10;
            returnButton = new Button(isOriginAtCenter: false, buttonPack)
            {
                Text = BACK_STRING,
                Dimensions = BACK_BUTTON_SIZE,
                Position = new Vector2(Constants.SCREEN_WIDTH - BACK_BUTTON_SIZE.X - returnButtonMargin, returnButtonMargin)
            };

            returnButton.Click += ReturnButton_Click;

            components = new List<Control>
            {
                returnButton
            };
        }

        #endregion

        #region Event Handlers

        private void ReturnButton_Click(object sender, System.EventArgs e)
        {
            game.ChangeState(new MenuState(game, graphicsDevice));
        }

        private void Preview_LoadRequested(object sender, ScenarioCore.ScenarioWrapper e)
        {
            if (preview.Scenario == null)
                return;
            
            game.ChangeState(new ScenarioState(game, graphicsDevice, e));
        }

        private void Spl_ScenarioSelected(object sender, XDocument e)
        {
            // Since user clicked on a scenario, pass it to the preview window
            preview.LoadPreview(loader.LoadScenario(e));
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            preview.Update(gameTime);
            spl.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            preview.Draw(gameTime, spriteBatch);
            spl.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        #endregion
    }
}
