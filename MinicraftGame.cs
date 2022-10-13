using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Font;
using Minicraft.Game.BlockType;
using Minicraft.Game.ItemType;
using Minicraft.Input;
using Minicraft.Scenes;
using Minicraft.Texture;
using Minicraft.Utils;

namespace Minicraft
{
    public class MinicraftGame : Microsoft.Xna.Framework.Game
    {
        public const string TITLE = "Minicraft";

        public static new GraphicsDevice GraphicsDevice { get; private set; }

        private static MinicraftGame _instance;
        private static AbstractScene _scene = new MainMenuScene();

        public MinicraftGame()
        {
            _instance = this;
            Display.Constructor(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / Display.FRAMES_PER_SECOND);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>((sender, eventArgs) => Display.UpdateSize(Window.ClientBounds.Width, Window.ClientBounds.Height));
        }

        protected override void Initialize()
        {
            // set window title
            Window.Title = TITLE;
            // set window properties
            Window.AllowAltF4 = false;
            // initialize display
            Display.Initialize();
            // base call
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GraphicsDevice = base.GraphicsDevice;
            Textures.Initialize();
            Fonts.Initialize(Content);
            Audio.Initialize(Content);
            Display.LoadContent();
            // initialize blocks and items (after textures)
            Blocks.Initialize();
            Items.Initialize();
            // base call
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // update input
            InputManager.Update();
            // togglable fullscreen
            if (Keybinds.Fullscreen.PressedThisFrame)
                Display.ToggleFullscreen();
            // update scene
            _scene.Update(gameTime);
            // base call
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // fill background
            GraphicsDevice.Clear(_scene.BackgroundColor);
            // begin drawing
            Display.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // draw scene
            _scene.Draw(gameTime);
            // end drawing
            Display.SpriteBatch.End();
            // base call
            base.Draw(gameTime);
        }

        public static void SetScene(AbstractScene scene) => _scene = scene;

        public static void EndProgram() => _instance.Exit();
    }
}
