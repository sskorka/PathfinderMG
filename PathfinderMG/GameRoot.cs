using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_Textbox;
using PathfinderMG.Core.Source;
using PathfinderMG.Core.Source.States;

namespace PathfinderMG.Core
{
    class GameRoot : Game
    {
        public static ContentManager ContentMgr;
        public static MouseManager Mouse;
        public static GraphicsDevice RootGraphicsDevice;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private State currentState, nextState;
        private Texture2D globalBackground;

        public GameRoot()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            KeyboardInput.Initialize(this, 500f, 20);
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = Constants.SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = Constants.SCREEN_HEIGHT;
            graphics.ApplyChanges();

            Mouse = new MouseManager();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentMgr = Content;
            RootGraphicsDevice = GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            currentState = new MenuState(this, graphics.GraphicsDevice);
            globalBackground = Content.Load<Texture2D>("Images/TitleBackground");
        }

        public void ChangeState(State state)
        {
            nextState = state;
        }

        protected override void Update(GameTime gameTime)
        {
            if (nextState != null)
            {
                currentState = nextState;
                nextState = null;
            }

            KeyboardInput.Update();
            Mouse.Update();
            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            spriteBatch.Begin();
            spriteBatch.Draw(globalBackground, Constants.SCREEN_RECT, Color.White);
            spriteBatch.End();

            currentState.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }
    }
}
