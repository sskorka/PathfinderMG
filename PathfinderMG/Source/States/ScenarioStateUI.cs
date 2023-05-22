using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_Textbox;
using PathfinderMG.Core.Source.GUI;
using PathfinderMG.Core.Source.GUI.Controls;
using PathfinderMG.Core.Source.ScenarioCore.Pathfinders;
using System.Collections.Generic;

namespace PathfinderMG.Core.Source.States
{
    partial class ScenarioState
    {
        private readonly Dictionary<string, string> LABEL_STRINGS = new Dictionary<string, string>()
        {
            { "loc", "Location: " },
            { "gcost", "G Cost: " },
            { "hcost", "H Cost: " },
            { "fcost", "F Cost: " },
            { "in", "Currently in: " },
            { "visSpeed", "Visualization Speed:" }
        };
        private readonly Dictionary<string, string> BUTTON_STRINGS = new Dictionary<string, string>()
        {
            { "start", "Run" },
            { "stop", "Stop" },
            { "reset", "Reset" },
            { "cancel", "Cancel" }
        };
        private readonly Dictionary<string, string> CHECKBOX_STRINGS = new Dictionary<string, string>()
        {
            { "allowDiagonalPathing", "Allow diagonal pathing" },
            { "instantPathing", "Instant pathing"},
            { "drawOpenClosed", "Draw open/closed nodes"}
        };

        private void LoadUI()
        {
            components = new List<Control>();

            // Load "Back to menu" button
            SpriteFont buttonFont = GameRoot.ContentMgr.Load<SpriteFont>("Fonts/DefaultFont");
            ButtonPack buttonPack = new ButtonPack()
            {
                TexDefault = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BackButton"),
                TexHovered = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BackButtonHovered"),
                TexSelected = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/BackButtonSelected")
            };

            int returnButtonMargin = 10;
            Vector2 returnButtonDimensions = new Vector2(80, 40);
            returnButton = new Button(isOriginAtCenter: false, buttonPack)
            {
                Text = "Back",
                Dimensions = returnButtonDimensions,
                Position = new Vector2(Constants.SCREEN_WIDTH - returnButtonDimensions.X - returnButtonMargin, returnButtonMargin)
            };

            returnButton.Click += ReturnButton_Click;

            // Load Options Panel
            string panelTitle = "Options";
            Vector2 panelAnchor = new Vector2(100, 50);
            Color panelFontColor = Color.White;
            Color panelBgColor = new Color(20, 20, 20);

            uiOptionsPanel = new PanelContainer(panelAnchor, panelTitle, panelFontColor, panelBgColor)
            {
                ControlMargin = 15,
                Padding = new Vector2(10, 10),
                Moveable = true
            };

            ConstructButtons_OptionsPanel(buttonFont);
            ConstructLabels_OptionsPanel();
            ConstructTextBox_OptionsPanel(buttonFont);
            ConstructCheckboxes_OptionsPanel();

            // Load Node Details Panel
            panelTitle = "Node information";
            panelAnchor = new Vector2(Constants.SCREEN_WIDTH - 220, 60);

            uiNodeInfoPanel = new PanelContainer(panelAnchor, panelTitle, panelFontColor, panelBgColor)
            {
                ControlMargin = 15,
                Padding = new Vector2(10, 10),
                Moveable = true
            };

            ConstructLabels_NodeInfoPanel();

            components.Add(uiOptionsPanel);
            components.Add(uiNodeInfoPanel);
            components.Add(returnButton);
        }

        private void ConstructLabels_NodeInfoPanel()
        {
            Label location, gCost, hCost, fCost, inList;

            location = new Label(LABEL_STRINGS["loc"]);
            gCost = new Label(LABEL_STRINGS["gcost"]);
            hCost = new Label(LABEL_STRINGS["hcost"]);
            fCost = new Label(LABEL_STRINGS["fcost"]);
            inList = new Label(LABEL_STRINGS["in"]);

            uiNodeInfoPanel.Add(location);
            uiNodeInfoPanel.Add(gCost);
            uiNodeInfoPanel.Add(hCost);
            uiNodeInfoPanel.Add(fCost);
            uiNodeInfoPanel.Add(inList);

            SetNodeInfo();
        }

        private void ConstructButtons_OptionsPanel(SpriteFont buttonFont)
        {
            ButtonPack buttonPack = new ButtonPack()
            {
                TexDefault = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/Button"),
                TexHovered = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/ButtonHovered"),
                TexSelected = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/ButtonSelected")
            };

            Button startPathfinder = new Button(isOriginAtCenter: false, buttonPack)
            {
                Text = BUTTON_STRINGS["start"]
            };

            startPathfinder.Click += StartPathfinder_Click;

            Button resetPathfinder = new Button(isOriginAtCenter: false, buttonPack)
            {
                Text = BUTTON_STRINGS["reset"]
            };

            resetPathfinder.Click += ResetPathfinder_Click;

            uiOptionsPanel.Add(startPathfinder);
            uiOptionsPanel.Add(resetPathfinder);
        }

        private void ConstructCheckboxes_OptionsPanel()
        {
            Checkbox diagonalsCheckbox = new Checkbox(CHECKBOX_STRINGS["allowDiagonalPathing"], Color.White)
            {
                IsChecked = pathfinder.AllowDiagonalMovement
            };

            diagonalsCheckbox.Click += DiagonalsCheckbox_Click;

            Checkbox instantCheckbox = new Checkbox(CHECKBOX_STRINGS["instantPathing"], Color.White)
            {
                IsChecked = pathfinder.InstantPathing
            };

            instantCheckbox.Click += InstantCheckbox_Click;

            Checkbox drawOpenClosedListsCheckbox = new Checkbox(CHECKBOX_STRINGS["drawOpenClosed"], Color.White)
            {
                IsChecked = scenarioGrid.DrawOpenClosedNodes,
                IsVisible = instantCheckbox.IsChecked
            };

            drawOpenClosedListsCheckbox.Click += DrawOpenClosedListsCheckbox_Click;

            uiOptionsPanel.Add(diagonalsCheckbox);
            uiOptionsPanel.Add(instantCheckbox);
            uiOptionsPanel.Add(drawOpenClosedListsCheckbox);
        }

        private void ConstructLabels_OptionsPanel()
        {
            Label visTimeLabel = new Label(LABEL_STRINGS["visSpeed"]);

            uiOptionsPanel.Add(visTimeLabel);
        }

        private void ConstructTextBox_OptionsPanel(SpriteFont font)
        {
            // Experimental textbox
            Rectangle tbArea = new Rectangle(0, 0, (int)font.MeasureString("999").X * 3, 20);

            TextBox visualizationSpeedInput = new TextBox(tbArea, MaxVisualizationTime.ToString().Length,
                                                          DefaultVisualizationTime.ToString(), GameRoot.RootGraphicsDevice, font,
                                                          TextBox.InputType.NumbersOnly, Color.White, Color.Gray, Color.DimGray, 30)
            {
                Rectangle = new Rectangle(tbArea.X, tbArea.Y, tbArea.Width, tbArea.Height),
                MarginTop = -15
            };

            visualizationSpeedInput.Renderer.Color = Color.White;
            visualizationSpeedInput.Cursor.Selection = new Color(Color.Purple, .4f);
            visualizationSpeedInput.Active = true;

            visualizationSpeedInput.EnterDown += VisualizationSpeedInput_EnterDown;
            visualizationSpeedInput.InputChanged += VisualizationSpeedInput_InputChanged;

            uiOptionsPanel.Add(visualizationSpeedInput);
        }
    }
}
