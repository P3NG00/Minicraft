using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game.Data;
using Game.Data.Scenes;

namespace Game
{
    public class Minicraft : Microsoft.Xna.Framework.Game
    {
        private const string TITLE = "Minicraft";

        private Scene _scene = new GameScene(WorldGen.GenerateWorld()); // TODO change to pass in custom generated world

        public Minicraft()
        {
            Display.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / Display.FRAMES_PER_SECOND);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>((sender, eventArgs) =>
            {
                Display.WindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
                Display.UpdateWindowSize();
            });
        }

        protected override void LoadContent()
        {
            // create square for drawing
            Display.TextureSquare = new Texture2D(GraphicsDevice, 1, 1);
            Display.TextureSquare.SetData(new[] {Color.White});
            // load font
            Display.Font = Content.Load<SpriteFont>("type_writer");
            // create display handler
            Display.SpriteBatch = new SpriteBatch(GraphicsDevice);
            // base call
            base.LoadContent();
        }

        protected override void Initialize()
        {
            // set window title
            Window.Title = TITLE;
            // set window properties
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Window.AllowAltF4 = false;
            Display.UpdateWindowSize();
            // base call
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // update input
            Input.Update();
            // handle input
            if (Input.KeyFirstDown(Keys.End))
                Exit();
            if (Input.KeyFirstDown(Keys.F1))
                Window.IsBorderless = !Window.IsBorderless;
            // update scene
            _scene.Update(gameTime);
            // base call
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // fill background
            GraphicsDevice.Clear(Colors.Background);
            // begin drawing
            Display.SpriteBatch.Begin();
            // draw scene
            _scene.Draw(gameTime);
            // end drawing
            Display.SpriteBatch.End();
            // base call
            base.Draw(gameTime);
        }
    }
}
