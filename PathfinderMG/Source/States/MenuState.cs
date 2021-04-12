using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfinderMG.Core.Source.GUI;
using PathfinderMG.Core.Source.GUI.Controls;
using PathfinderMG.Core.Source.GUI.Controls.Dialogs;
using System;
using System.Collections.Generic;

namespace PathfinderMG.Core.Source.States
{
    class MenuState : State
    {
        private readonly Dictionary<string, string> BUTTON_STRINGS = new Dictionary<string, string>()
        {
            { "load", "Load Scenario" },
            { "editor", "Scenario Editor\n(Coming soon!)" },
            { "exit", "Exit" }
        };
        private readonly Vector2 DEFAULT_BUTTON_SIZE = new Vector2(160, 40);

        private EditorEntryDialog editorEntryDialog = new EditorEntryDialog();
        private bool showEditorEntryDialog = false;

        public MenuState(GameRoot game, GraphicsDevice graphicsDevice) 
            : base(game, graphicsDevice)
        {

            editorEntryDialog.BackgroundClick += EditorEntryDialog_BackgroundClick;
            editorEntryDialog.NewScenarioButton.Click += ScenarioEditor_NewScenario;

            ConstructButtons();
        }

        #region Methods

        private void ConstructButtons()
        {
            SpriteFont buttonFont = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
            ButtonPack buttonPackDefault = new ButtonPack()
            {
                TexDefault = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/Button"),
                TexHovered = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/ButtonHovered"),
                TexSelected = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/ButtonSelected")
            };
            ButtonPack buttonPackBig = new ButtonPack()
            {
                TexDefault = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BigButton"),
                TexHovered = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BigButtonHovered"),
                TexSelected = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BigButtonSelected")
            };

            Vector2 buttonsAnchor = new Vector2(Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2);
            
            Button loadScenarioButton = new Button(isOriginAtCenter: true, buttonPackDefault)
            {
                Position = buttonsAnchor + new Vector2(0, -60),
                Dimensions = DEFAULT_BUTTON_SIZE,
                Text = BUTTON_STRINGS["load"]
            };

            loadScenarioButton.Click += LoadScenarioButton_Click;

            Button scenarioEditorButton = new Button(isOriginAtCenter: true, buttonPackBig)
            {
                Position = buttonsAnchor + new Vector2(0, 0),
                Dimensions = DEFAULT_BUTTON_SIZE + new Vector2(0, 20), // Temporary +(0, 20) while it says "Coming Soon"
                Text = BUTTON_STRINGS["editor"]
            };

            // Deactivated for v0.1.0
            // scenarioEditorButton.Click += ScenarioEditorButton_Click;

            Button exitButton = new Button(isOriginAtCenter: true, buttonPackDefault)
            {
                Position = buttonsAnchor + new Vector2(0, 60),
                Dimensions = DEFAULT_BUTTON_SIZE,
                Text = BUTTON_STRINGS["exit"]
            };

            exitButton.Click += ExitButton_Click;

            components = new List<Control>()
            {
                loadScenarioButton,
                scenarioEditorButton,
                exitButton
            };
        }

        #endregion

        #region Event Handlers

        private void LoadScenarioButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new LoadState(game, graphicsDevice, editorMode: false));
        }

        private void ScenarioEditorButton_Click(object sender, EventArgs e)
        {
            showEditorEntryDialog = true;
        }

        private void EditorEntryDialog_BackgroundClick(object sender, EventArgs e)
        {
            showEditorEntryDialog = false;
        }

        private void ScenarioEditor_NewScenario(object sender, EventArgs e)
        {
            game.ChangeState(new EditorState(game, graphicsDevice));
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            if (showEditorEntryDialog)
                editorEntryDialog.Update(gameTime);
            else
                base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
                        
            base.Draw(gameTime, spriteBatch);

            if (showEditorEntryDialog)
                editorEntryDialog.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        #endregion
    }
}
