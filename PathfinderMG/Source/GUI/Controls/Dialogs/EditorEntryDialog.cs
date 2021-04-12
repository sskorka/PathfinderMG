using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PathfinderMG.Core.Source.GUI.Controls.Dialogs
{
    class EditorEntryDialog : DialogBase
    {
        #region Fields

        private const string editExistingStr = "Edit existing scenario";
        private const string newScenarioStr = "Create new scenario";
        private const int buttonWidth = 200;
        private const int buttonHeight = 50;

        private Texture2D dialogTex;

        #endregion

        #region Properties

        public int VerticalButtonMargin { get; set; } = 10;
        public int VerticalPadding { get; set; } = 30;
        public int HorizontalPadding { get; set; } = 30;
        public Button EditScenarioButton;
        public Button NewScenarioButton;

        #endregion

        #region Constructors

        public EditorEntryDialog()
            : base(isCancellableThroughBackground: true)
        {
            dialogTex = GameRoot.ContentMgr.Load<Texture2D>("Controls/Button");

            Dimensions = new Vector2(buttonWidth + (2 * HorizontalPadding), (2 * buttonHeight) + (2 * VerticalPadding) + VerticalButtonMargin);
            Position = new Vector2(Constants.SCREEN_WIDTH / 2 - Dimensions.X / 2, Constants.SCREEN_HEIGHT / 2 - Dimensions.Y / 2);

            LoadUI();
        }

        #endregion

        #region Methods

        private void LoadUI()
        {
            EditScenarioButton = new Button(isOriginAtCenter: true)
            {
                Text = editExistingStr,
                Dimensions = new Vector2(buttonWidth, buttonHeight)
            };

            EditScenarioButton.Position = new Vector2(Position.X + Dimensions.X / 2, Position.Y + (Dimensions.Y / 2) - (EditScenarioButton.Dimensions.Y / 2) - VerticalButtonMargin);
            //EditScenarioButton.Click += EditScenarioButton_Click;

            NewScenarioButton = new Button(isOriginAtCenter: true)
            {
                Text = newScenarioStr,
                Dimensions = new Vector2(buttonWidth, buttonHeight)
            };

            NewScenarioButton.Position = new Vector2(Position.X + Dimensions.X / 2, Position.Y + (Dimensions.Y / 2) + (EditScenarioButton.Dimensions.Y / 2) + VerticalButtonMargin);
            //NewScenarioButton.Click += NewScenarioButton_Click;

            components = new List<Control>()
            {
                EditScenarioButton,
                NewScenarioButton
            };
        }

        #endregion

        #region Event Handlers

        private void NewScenarioButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EditScenarioButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Update/Draw

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(dialogTex, Rectangle, Color.White);

            foreach (var c in components)
                c.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
