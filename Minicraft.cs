using System;
using System.Linq;
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

        // tick & frame handling variables
        public static int Ticks => _ticks[0];
        public static float AverageFramesPerSecond => _lastFps.Average();
        public static float AverageTicksPerFrame => (float)_lastTickDifferences.Average();
        private static int[] _ticks = new [] {0, 0};
        private static int[] _lastTickDifferences = new int[World.TICKS_PER_SECOND];
        private static float[] _lastFps = new float[Display.FRAMES_PER_SECOND];
        private static float _tickDelta = 0f;

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
            // update ticks
            UpdateTicks((float)gameTime.ElapsedGameTime.TotalSeconds * Debug.TimeScale);
            // update scene
            _scene.Update(gameTime);
            // tick scene
            while (GameTick())
                _scene.Tick();
            // base call
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // fill background
            GraphicsDevice.Clear(_scene.BackgroundColor);
            // begin drawing
            Display.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // update frames per second
            UpdateFramesPerSecond((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            // draw scene
            _scene.Draw(gameTime);
            // end drawing
            Display.SpriteBatch.End();
            // base call
            base.Draw(gameTime);
        }

        private bool GameTick()
        {
            if (_tickDelta < World.TICK_STEP)
                return false;
            // decrement delta time by tick step
            _tickDelta -= World.TICK_STEP;
            // increment tick counter
            _ticks[0]++;
            // return success
            return true;
        }

        private void UpdateTicks(float timeThisUpdate)
        {
            // add delta time
            _tickDelta += timeThisUpdate;
            // move last tick count down
            for (int i = _lastTickDifferences.Length - 2; i >= 0; i--)
                _lastTickDifferences[i + 1] = _lastTickDifferences[i];
            // set last tick difference
            _lastTickDifferences[0] = _ticks[0] - _ticks[1];
            // update last tick count
            _ticks[1] = _ticks[0];
        }

        private void UpdateFramesPerSecond(float timeThisFrame)
        {
            // move values down
            for (int i = _lastFps.Length - 2; i >= 0; i--)
                _lastFps[i + 1] = _lastFps[i];
            // store fps value
            _lastFps[0] = 1000f / timeThisFrame;
        }

        public static void AddTick() => _tickDelta += World.TICK_STEP;

        public static void SetScene(AbstractScene scene) => _instance._nextScene = scene;

        public static void EndProgram() => _instance.Exit();
    }
}
