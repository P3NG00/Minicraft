using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game.Data;

namespace Game
{
    public class Minicraft : Microsoft.Xna.Framework.Game
    {
        // constants
        private const string TITLE = "Minicraft";
        private const float PLAYER_SPEED = 5f;
        private const float PLAYER_JUMP = 3.5f;
        private const int UI_SPACER = 5;

        private readonly Vector2 PlayerSize = new Vector2(1.8f, 2.8f);

        // monogame
        private GraphicsDeviceManager _graphics;

        // variables
        private Entity _player;
        private World _world;
        private float _tickDelta = 0f;
        private Vector2 _mouseBlock;
        private Point _mouseBlockInt;
        private Block _currentBlock = Blocks.Dirt;
        private int[] _ticks = new [] {0, 0};
        private int[] _lastTickDifferences = new int[10];
        private float[] _lastFps = new float[10];

        public Minicraft()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / Display.FRAMES_PER_SECOND);
            IsMouseVisible = true;
            // TODO handle resizing and detecting
            // Window.AllowUserResizing = true;
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
            // create world
            _world = WorldGen.GenerateWorld();
            // create player
            _player = new Entity(Colors.Player, PlayerSize, PLAYER_SPEED, PLAYER_JUMP);
            var playerX = (int)(_world.Width / 2f);
            _player.Position = new Vector2(playerX, Math.Max(_world.GetTopBlock(playerX - 1).y, _world.GetTopBlock(playerX).y) + 1);
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
            _graphics.PreferredBackBufferWidth = Display.WindowSize.X;
            _graphics.PreferredBackBufferHeight = Display.WindowSize.Y;
            _graphics.ApplyChanges();
            // base call
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // add delta time
            _tickDelta += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // move last tick count down
            for (int i = _lastTickDifferences.Length - 2; i >= 0; i--)
                _lastTickDifferences[i + 1] = _lastTickDifferences[i];
            // set last tick difference
            _lastTickDifferences[0] = _ticks[0] - _ticks[1];
            // update last tick count
            _ticks[1] = _ticks[0];
            // update for every tick step
            while (_tickDelta >= World.TickStep)
            {
                // decrement delta time by tick step
                _tickDelta -= World.TickStep;
                // increment tick counter
                _ticks[0]++;
                // clear previously updated positions
                Debug.UpdatedPoints.Clear();
                // update input
                Input.Update();
                // handle input
                if (Input.KeyFirstDown(Keys.End))
                    Exit();
                if (Input.KeyFirstDown(Keys.Tab))
                    Display.ShowGrid = !Display.ShowGrid;
                if (Input.KeyFirstDown(Keys.F1))
                    Window.IsBorderless = !Window.IsBorderless;
                if (Debug.Enabled && Input.KeyFirstDown(Keys.F11))
                    Debug.TrackUpdated = !Debug.TrackUpdated;
                if (Input.KeyFirstDown(Keys.F12))
                    Debug.Enabled = !Debug.Enabled;
                if (Input.KeyFirstDown(Keys.D1))
                    _currentBlock = Blocks.Dirt;
                if (Input.KeyFirstDown(Keys.D2))
                    _currentBlock = Blocks.Grass;
                if (Input.KeyFirstDown(Keys.D3))
                    _currentBlock = Blocks.Stone;
                if (Input.KeyFirstDown(Keys.D4))
                    _currentBlock = Blocks.Wood;
                if (Input.KeyFirstDown(Keys.D5))
                    _currentBlock = Blocks.Leaves;
                Display.BlockScale = Math.Clamp(Display.BlockScale + Input.ScrollWheel, Display.BLOCK_SCALE_MIN, Display.BLOCK_SCALE_MAX);
                // get block position from mouse
                var mousePos = Input.MousePosition.ToVector2();
                mousePos.Y = Display.WindowSize.Y - mousePos.Y - 1;
                _mouseBlock = ((mousePos - (Display.WindowSize.ToVector2() / 2f)) / Display.BlockScale) + (_player.Position + new Vector2(0, _player.Dimensions.Y / 2f));
                _mouseBlockInt = _mouseBlock.ToPoint();
                // catch out of bounds
                if (_mouseBlockInt.X >= 0 && _mouseBlockInt.X < _world.Width &&
                    _mouseBlockInt.Y >= 0 && _mouseBlockInt.Y < _world.Height)
                {
                    if (Input.ButtonLeftFirstDown())
                        _world.Block(_mouseBlockInt) = Blocks.Air;
                    else if (Input.ButtonRightFirstDown())
                        _world.Block(_mouseBlockInt) = _currentBlock;
                    else if (Input.ButtonMiddleFirstDown())
                        _world.Block(_mouseBlockInt).Update(_mouseBlockInt, _world);
                }
                // update world
                _world.Update();
                // update player
                _player.Update(_world);
                // update display handler
                Display.Update(_player);
            }
            // base call
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // store fps value
            for (int i = _lastFps.Length - 2; i >= 0; i--)
                _lastFps[i + 1] = _lastFps[i];
            _lastFps[0] = 1000f / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            // fill background
            GraphicsDevice.Clear(Colors.Background);
            // begin drawing
            Display.SpriteBatch.Begin();
            // draw world
            _world.Draw(_player);
            // draw player
            _player.Draw();
            // draw ui
            var drawPos = new Vector2(UI_SPACER, Display.WindowSize.Y - Display.Font.LineSpacing - UI_SPACER);
            Display.DrawString(drawPos, $"current block: {_currentBlock.Name}", Colors.FontUI);
            // draw debug
            if (Debug.Enabled)
            {
                drawPos = new Vector2(UI_SPACER);
                foreach (var debugInfo in new[] {
                    $"window_size: {Display.WindowSize.X}x{Display.WindowSize.Y}",
                    $"world_size: {_world.Width}x{_world.Height}",
                    $"show_grid: {Display.ShowGrid}",
                    $"time: {(_ticks[0] / (float)World.TICKS_PER_SECOND):0.000}",
                    $"ticks: {_ticks[0]} ({World.TICKS_PER_SECOND} ticks/sec)",
                    $"frames_per_second: {_lastFps.Average():0.000}",
                    $"ticks_per_frame: {_lastTickDifferences.Average():0.000}",
                    $"x: {_player.Position.X:0.000}",
                    $"y: {_player.Position.Y:0.000}",
                    $"block_scale: {Display.BlockScale}",
                    $"mouse_x: {_mouseBlock.X:0.000} ({_mouseBlockInt.X})",
                    $"mouse_y: {_mouseBlock.Y:0.000} ({_mouseBlockInt.Y})",
                    $"player_velocity: {_player.Velocity.Length():0.000}",
                    $"player_grounded: {_player.IsGrounded}"})
                {
                    Display.DrawString(drawPos, debugInfo, Colors.FontDebug);
                    drawPos.Y += UI_SPACER + Display.Font.LineSpacing;
                }
            }
            // end drawing
            Display.SpriteBatch.End();
            // base call
            base.Draw(gameTime);
        }
    }
}
