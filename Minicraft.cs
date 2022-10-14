using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinicraftGame.Font;
using MinicraftGame.Game.BlockType;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.ItemType;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Input;
using MinicraftGame.Scenes;
using MinicraftGame.Texture;
using MinicraftGame.Utils;

namespace MinicraftGame
{
    public class Minicraft : Microsoft.Xna.Framework.Game
    {
        public const string TITLE = "Minicraft";

        public static new GraphicsDevice GraphicsDevice { get; private set; }

        public static PlayerEntity Player = null;
        public static World World = null;

        private static Minicraft _instance;
        private AbstractScene _scene = new MainMenuScene();
        private AbstractScene _nextScene = null;

        public Minicraft()
        {
            _instance = this;
            Display.CreateGraphicsManager(this);
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
            // update scene if set
            if (_nextScene != null)
            {
                _scene = _nextScene;
                _nextScene = null;
            }
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

        public static void SetScene(AbstractScene scene) => _instance._nextScene = scene;

        public static void EndProgram() => _instance.Exit();
    }
}
