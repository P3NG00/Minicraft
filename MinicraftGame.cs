using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minicraft.Scenes;
using Minicraft.Utils;

namespace Minicraft
{
    public class MinicraftGame : Microsoft.Xna.Framework.Game
    {
        public const string TITLE = "Minicraft";

        private static MinicraftGame _instance;
        private static IScene _scene = new MainMenuScene();

        public MinicraftGame()
        {
            _instance = this;
            Display.Initialize(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / Display.FRAMES_PER_SECOND);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>((sender, eventArgs) => Display.WindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height));
        }

        protected override void LoadContent()
        {
            Display.LoadContent(GraphicsDevice, Content);
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
            // fix initial window size
            Display.UpdateWindowSize();
            // base call
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // update input
            Input.Update();
            // TODO replace with fullscreen keybind
            if (Input.KeyFirstDown(Keys.F10))
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

        public static void SetScene(IScene scene) => _scene = scene;

        public static void EndProgram() => _instance.Exit();
    }
}
