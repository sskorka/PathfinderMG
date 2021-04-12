using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfinderMG.Core.Source.GUI;
using PathfinderMG.Core.Source.GUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PathfinderMG.Core.Source.ScenarioLoader
{
    class ScenarioPaginatedList
    {
        #region Fields

        private const int ITEMS_PER_PAGE = 6;

        private int numberOfPages, currentPage;
        private Dictionary<string, XDocument> allScenarios = new Dictionary<string, XDocument>();
        private List<PaginatedListItem> scenariosDisplayed = new List<PaginatedListItem>();
        private ScenarioLoader loader;

        // UI Elements
        private List<Control> components;
        private Button pageLeft, pageRight;
        private Label currentPageLabel, titleLabel;

        #endregion

        #region Properties

        public Texture2D Texture { get; set; }
        public Rectangle Area { get; set; }
        public Color BackgroundColor { get; set; }

        // UI Elements
        public int ButtonSize { get; set; } = 35;
        public int ButtonMargin { get; set; } = 50;
        public int TitleMargin { get; set; } = 20;
        public int ListItemsSpacing { get; set; } = 10;

        public event EventHandler<XDocument> ScenarioSelected;

        #endregion

        #region Constructor

        public ScenarioPaginatedList(Rectangle area, Color bgColor, ScenarioLoader loader)
        {
            Area = area;
            BackgroundColor = bgColor;
            this.loader = loader;

            LoadUI();
        }

        #endregion

        #region Methods

        public void LoadUI()
        {
            Texture = GameRoot.ContentMgr.Load<Texture2D>("Controls/PanelBackground");
            ButtonPack buttonPack = new ButtonPack()
            {
                TexDefault = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/SquareButton"),
                TexHovered = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/SquareButtonHovered"),
                TexSelected = GameRoot.ContentMgr.Load<Texture2D>("Controls/Buttons/SquareButtonSelected")
            };

            pageLeft = new Button(isOriginAtCenter: true, buttonPack)
            {
                Text = "<",
                Position = new Vector2(Area.X + ButtonMargin, Area.Y + Area.Height - ButtonMargin),
                Dimensions = new Vector2(ButtonSize),
                IsEnabled = false
            };

            pageLeft.Click += PageLeft_Click;

            pageRight = new Button(isOriginAtCenter: true, buttonPack)
            {
                Text = ">",
                Position = new Vector2(Area.X + Area.Width - ButtonMargin, Area.Y + Area.Height - ButtonMargin),
                Dimensions = new Vector2(ButtonSize),
                IsEnabled = false
            };

            pageRight.Click += PageRight_Click;

            currentPageLabel = new Label("Page 1")
            {
                Position = new Vector2(Area.X + (Area.Width / 2), Area.Y + Area.Height - ButtonMargin)
            };

            // Label needs to be constructed first, so that its dimensions get calculated internally
            currentPageLabel.Position -= new Vector2(currentPageLabel.Dimensions.X / 2, currentPageLabel.Dimensions.Y / 2);

            titleLabel = new Label("Select scenario")
            {
                Position = new Vector2(Area.X + TitleMargin, Area.Y + TitleMargin)
            };

            components = new List<Control>()
            {
                pageLeft,
                pageRight,
                currentPageLabel,
                titleLabel
            };
        }

        /// <summary>
        /// Call this only once to load the scenarios
        /// </summary>
        public void LoadScenarios(Dictionary<string, XDocument> scenarios)
        {
            allScenarios = scenarios;

            // Divide into pages and populate the first
            numberOfPages = (allScenarios.Count + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE;
            currentPage = 1;
            SwitchPage(currentPage);

            // If there's more than 1 page, activate the '>' button
            if (numberOfPages > 1)
                pageRight.IsEnabled = true;
        }

        private void SwitchPage(int page)
        {
            // Check if buttons needs (de)activation
            pageLeft.IsEnabled = currentPage > 1;
            pageRight.IsEnabled = currentPage < numberOfPages;

            currentPageLabel.Text = $"Page { page }";

            PopulatePage(page);
        }

        private void PopulatePage(int page)
        {
            int newItemX = PaginatedListItem.ContainerArea.X + PaginatedListItem.MarginHorizontal;
            int newItemY = PaginatedListItem.ContainerArea.Y + PaginatedListItem.MarginVertical;

            scenariosDisplayed.Clear();

            for (int i = (page - 1) * ITEMS_PER_PAGE; i < ITEMS_PER_PAGE * page && i < allScenarios.Count; i++)
            {
                // Clamp i: 0 < i < ITEMS_PER_PAGE
                int j = i % ITEMS_PER_PAGE;
                XElement root = allScenarios.ElementAt(i).Value.Root;
                PaginatedListItem newListItem = new PaginatedListItem(root.Element("Title").Value, root.Element("Author").Value)
                {
                    Position = (i == 0 || i % ITEMS_PER_PAGE == 0) 
                                        ? new Vector2(newItemX, newItemY)
                                        : new Vector2(newItemX, newItemY + (ListItemsSpacing * j) + scenariosDisplayed[j - 1].Area.Height * j),
                    ScenarioFile = allScenarios.ElementAt(i).Value
                };
                newListItem.ItemSelected += NewListItem_ItemSelected;
                scenariosDisplayed.Add(newListItem);
            }
        }

        #endregion

        #region Event Handlers

        private void PageRight_Click(object sender, EventArgs e)
        {
            SwitchPage(++currentPage);
        }

        private void PageLeft_Click(object sender, EventArgs e)
        {
            SwitchPage(--currentPage);
        }

        private void NewListItem_ItemSelected(object sender, EventArgs e)
        {
            // Deselect others
            scenariosDisplayed.ForEach(s => s.IsSelected = false);

            ScenarioSelected?.Invoke(this, (sender as PaginatedListItem).ScenarioFile);
        }

        #endregion

        #region Update/Draw

        public void Update(GameTime gameTime)
        {
            foreach (var c in components)
                c.Update(gameTime);

            foreach (var item in scenariosDisplayed)            
                item.Update(gameTime);            
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Area, BackgroundColor);
            foreach (var c in components)
                c.Draw(gameTime, spriteBatch);

            foreach (var item in scenariosDisplayed)
                item.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
