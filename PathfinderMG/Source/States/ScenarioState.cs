using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_Textbox;
using PathfinderMG.Core.Source.GUI.Controls;
using PathfinderMG.Core.Source.ScenarioCore;
using PathfinderMG.Core.Source.ScenarioCore.Pathfinders;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PathfinderMG.Core.Source.States;

partial class ScenarioState : State
{
    #region Fields

    public const int MinVisualizationTime = 5;
    public const int MaxVisualizationTime = 100;
    public const int DefaultVisualizationTime = 80;

    private readonly Grid scenarioGrid;
    private readonly IPathfinder pathfinder;
    private readonly Dictionary<string, bool> focusModeSettings = new();
    private PanelContainer uiOptionsPanel, uiNodeInfoPanel;
    private Button returnButton;

    private CancellationTokenSource cts = new();

    #endregion

    #region Constructor

    public ScenarioState(GameRoot game, GraphicsDevice graphicsDevice, ScenarioWrapper scenario) 
        : base(game, graphicsDevice)
    {
        // Load grid
        Tuple<bool, Rectangle> previewInfo = Tuple.Create(false, Rectangle.Empty);
        scenarioGrid = new Grid(previewInfo, 50, scenario);

        // Load pathfinder class
        pathfinder = GameRoot.Services.GetService<IPathfinder>();
        pathfinder.SetupGrid(scenarioGrid, DefaultVisualizationTime);
        pathfinder.AllowDiagonalMovement = true;
        pathfinder.InstantPathing = false;
        pathfinder.PathfindingFinished += PathfindingFinished;

        // Configure grid
        scenarioGrid.AssignNewAlgorithm(pathfinder);
        scenarioGrid.NodeHovered += ScenarioGrid_NodeHovered;
        scenarioGrid.NodeLeft += ScenarioGrid_NodeLeft;

        // Load UI (ScenarioStateUI partial class)
        LoadUI();
    }

    #endregion

    #region Methods

    private void ReturnToMenu()
    {
        game.ChangeState(new LoadState(game, graphicsDevice, editorMode: false));
    }

    private void SetNodeInfo(string location = "-", string gCost = "-", string hCost = "-", string fCost = "-", string inList = "-")
    {
        uiNodeInfoPanel.Controls[0].Text = LABEL_STRINGS["loc"] + location;
        uiNodeInfoPanel.Controls[1].Text = LABEL_STRINGS["gcost"] + gCost;
        uiNodeInfoPanel.Controls[2].Text = LABEL_STRINGS["hcost"] + hCost;
        uiNodeInfoPanel.Controls[3].Text = LABEL_STRINGS["fcost"] + fCost;
        uiNodeInfoPanel.Controls[4].Text = LABEL_STRINGS["in"] + inList;
    }

    /// <summary>
    /// Hides all the option buttons.
    /// Should be enabled while algorithm is running.
    /// </summary>
    private void EnterFocusMode()
    {
        // User should not be able to change settings while algorithm is running

        focusModeSettings.Clear();
        
        // Save current settings
        foreach (var c in uiOptionsPanel.Controls)
            if (c is not Button && c is not TextBox)
                focusModeSettings.Add(c.Text, c.IsVisible);

        // Turn visibility off
        foreach (var c in uiOptionsPanel.Controls)            
            if (c is not Button)
                c.IsVisible = false;            
    }

    private void ExitFocusMode()
    {
        // Restore settings
        foreach (var c in uiOptionsPanel.Controls)
            if (c is not Button && c is not TextBox)
                c.IsVisible = focusModeSettings[c.Text];

        uiOptionsPanel.Controls.Find(c => c is TextBox).IsVisible = true;
    }

    #endregion

    #region Event Handlers

    private async void StartPathfinder_Click(object sender, EventArgs e)
    {
        // Prepare and start pathing
        pathfinder.ClearPath();
        (sender as Button).IsEnabled = false;
        cts = new CancellationTokenSource();
        EnterFocusMode();
        await pathfinder.FindPathAsync(cts.Token);
    }

    private void PathfindingFinished(object sender, EventArgs e)
    {
        uiOptionsPanel.Controls.Find(c => c.Text == BUTTON_STRINGS["start"]).IsEnabled = true;
        ExitFocusMode();
    }

    private void ResetPathfinder_Click(object sender, EventArgs e)
    {
        if(cts != null)
        {
            try
            {
                cts.Cancel();
            }
            catch(ObjectDisposedException)
            {
                System.Diagnostics.Debug.WriteLine("Attempted to cancel prematurely!");
            }
        }
        pathfinder.ClearPath();
        PathfindingFinished(this, e);
    }

    private void DiagonalsCheckbox_Click(object sender, EventArgs e)
    {
        if (sender is Checkbox)
            pathfinder.AllowDiagonalMovement = (sender as Checkbox).IsChecked;
    }

    private void InstantCheckbox_Click(object sender, EventArgs e)
    {
        if (sender is Checkbox)
        {
            bool enabled = (sender as Checkbox).IsChecked;

            pathfinder.InstantPathing = enabled;
            Checkbox drawNodesCheckbox = uiOptionsPanel.Controls.Find(c => c.Text == "Draw open/closed nodes") as Checkbox;
            drawNodesCheckbox.IsVisible = enabled;

            // If instant pathing gets turned off, make sure that drawing open/closed nodes is always true
            scenarioGrid.DrawOpenClosedNodes = !enabled || scenarioGrid.DrawOpenClosedNodes;
        }
    }

    private void DrawOpenClosedListsCheckbox_Click(object sender, EventArgs e)
    {
        scenarioGrid.DrawOpenClosedNodes = (sender as Checkbox).IsChecked;
    }

    private void ScenarioGrid_NodeHovered(object sender, Node node)
    {
        string location = $"[{ (int)node.Position.X }, { (int)node.Position.Y }]";

        SetNodeInfo(location, node.GCost.ToString(), node.HCost.ToString(), (node.GCost + node.HCost).ToString());
    }

    private void ScenarioGrid_NodeLeft(object sender, EventArgs e)
    {
        SetNodeInfo();
    }

    private void VisualizationSpeedInput_InputChanged(object sender, string e)
    {
        if (Int32.TryParse(e, out int inputValue))
        {
            if (inputValue > MaxVisualizationTime)
                inputValue = MaxVisualizationTime;

            pathfinder.VisualizationTime = (MaxVisualizationTime - inputValue) + MinVisualizationTime;
            (sender as TextBox).Text.String = inputValue.ToString();
        }
    }

    private void VisualizationSpeedInput_EnterDown(object sender, KeyboardInput.KeyEventArgs e)
    {
        (sender as TextBox).Active = false;
    }

    private void ReturnButton_Click(object sender, EventArgs e)
    {
        ReturnToMenu();
    }

    #endregion

    #region Update/Draw

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            ReturnToMenu();
        
        scenarioGrid.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        scenarioGrid.Draw(gameTime, spriteBatch);
        base.Draw(gameTime, spriteBatch);

        spriteBatch.End();
    }

    #endregion
}
