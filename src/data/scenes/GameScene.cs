using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data.Scenes
{
    public sealed class GameScene : Scene
    {
        private const int UI_SPACER = 5;

        private static readonly Vector2 BarSize = new Vector2(150, 30);

        private int Ticks => _ticks[0];
        private float AverageFramesPerSecond => _lastFps.Average();
        private float AverageTicksPerFrame => (float)_lastTickDifferences.Average();

        private float _tickDelta = 0f;
        private int[] _ticks = new [] {0, 0};
        private int[] _lastTickDifferences = new int[10];
        private float[] _lastFps = new float[10];

        private readonly Player _player;
        private readonly List<NPC> _npcList = new List<NPC>();
        private readonly World _world;

        private Block _currentBlock = Blocks.Dirt;
        private Vector2 _lastMouseBlock;
        private Point _lastMouseBlockInt;

        public GameScene()
        {
            // TODO change to accept world as passed in parameter for custom generated worlds
            _world = WorldGen.GenerateWorld();
            _player = new Player(_world);
        }

        public void Update(GameTime gameTime)
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
            // get block position from mouse
            var mousePos = Input.MousePosition.ToVector2();
            mousePos.Y = Display.WindowSize.Y - mousePos.Y - 1;
            _lastMouseBlock = ((mousePos - (Display.WindowSize.ToVector2() / 2f)) / Display.BlockScale) + (_player.Position + new Vector2(0, _player.Dimensions.Y / 2f));
            _lastMouseBlockInt = _lastMouseBlock.ToPoint();
            // handle input
            if (Input.KeyFirstDown(Keys.Escape))
                Minicraft.SetScene(new MainMenuScene());
            if (Input.KeyFirstDown(Keys.Tab))
                Display.ShowGrid = !Display.ShowGrid;
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
            // catch out of bounds
            if (_lastMouseBlockInt.X >= 0 && _lastMouseBlockInt.X < _world.Width &&
                _lastMouseBlockInt.Y >= 0 && _lastMouseBlockInt.Y < _world.Height)
            {
                bool ctrl = Input.KeyHeld(Keys.LeftControl) || Input.KeyHeld(Keys.RightControl);
                if (ctrl ? Input.MouseLeftFirstDown() : Input.MouseLeftHeld())
                    _world.Block(_lastMouseBlockInt) = Blocks.Air;
                if (ctrl ? Input.MouseRightFirstDown() : Input.MouseRightHeld())
                    _world.Block(_lastMouseBlockInt) = _currentBlock;
                if (Input.MouseMiddleFirstDown())
                    _npcList.Add(new NPC(_lastMouseBlock));
            }
            // update for every tick step
            while (Tick())
            {
                // clear previously updated positions
                Debug.UpdatedPoints.Clear();
                // update world
                _world.Update();
                // update player
                _player.Update(_world);
                // update npc's
                _npcList.ForEach(npc => npc.Update(_world));
            }
        }

        public void Draw(GameTime gameTime)
        {
            UpdateFramesPerSecond((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            // update display handler
            Display.Update(_player);
            // draw world
            _world.Draw(_player);
            // draw player
            _player.Draw();
            // draw npc's
            _npcList.ForEach(npc => npc.Draw());
            // draw ui
            // draw health bar
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (BarSize.X / 2f), Display.WindowSize.Y - BarSize.Y);
            Display.Draw(drawPos, BarSize, Colors.UI_Bar);
            // adjust size to fit within bar
            drawPos += new Vector2(UI_SPACER);
            var healthSize = BarSize - (new Vector2(UI_SPACER) * 2);
            // readjust size to display real health
            healthSize.X *= _player.Life / _player.MaxLife;
            Display.Draw(drawPos, healthSize, Colors.UI_Life);
            // draw health numbers on top of bar
            var healthString = $"{_player.Life:0.#}/{_player.MaxLife:0.#}";
            var textSize = Display.FontUI.MeasureString(healthString);
            drawPos = new Vector2((Display.WindowSize.X / 2f) - (textSize.X / 2f), Display.WindowSize.Y - 22);
            Display.DrawString(Display.FontUI, drawPos, healthString, Colors.UI_TextLife);
            // draw currently selected block
            drawPos = new Vector2(UI_SPACER, Display.WindowSize.Y - Display.FontUI.LineSpacing - UI_SPACER);
            Display.DrawString(Display.FontUI, drawPos, $"current block: {_currentBlock.Name}", Colors.UI_TextBlock);
            // draw debug
            if (Debug.Enabled)
            {
                drawPos = new Vector2(UI_SPACER);
                foreach (var debugInfo in new[] {
                    $"window_size: {Display.WindowSize.X}x{Display.WindowSize.Y}",
                    $"world_size: {_world.Width}x{_world.Height}",
                    $"show_grid: {Display.ShowGrid}",
                    $"time: {(Ticks / (float)World.TICKS_PER_SECOND):0.000}",
                    $"ticks: {Ticks} ({World.TICKS_PER_SECOND} ticks/sec)",
                    $"frames_per_second: {AverageFramesPerSecond:0.000}",
                    $"ticks_per_frame: {AverageTicksPerFrame:0.000}",
                    $"x: {_player.Position.X:0.000}",
                    $"y: {_player.Position.Y:0.000}",
                    $"block_scale: {Display.BlockScale}",
                    $"mouse_x: {_lastMouseBlock.X:0.000} ({_lastMouseBlockInt.X})",
                    $"mouse_y: {_lastMouseBlock.Y:0.000} ({_lastMouseBlockInt.Y})",
                    $"player_velocity: {_player.Velocity.Length() * _player.MoveSpeed:0.000}",
                    $"player_grounded: {_player.IsGrounded}"})
                {
                    Display.DrawString(Display.FontUI, drawPos, debugInfo, Colors.UI_TextDebug);
                    drawPos.Y += UI_SPACER + Display.FontUI.LineSpacing;
                }
            }
        }

        private bool Tick()
        {
            if (_tickDelta >= World.TickStep)
            {
                // decrement delta time by tick step
                _tickDelta -= World.TickStep;
                // increment tick counter
                _ticks[0]++;
                // return success
                return true;
            }
            return false;
        }

        private void UpdateFramesPerSecond(float timeThisFrame)
        {
            // move values down
            for (int i = _lastFps.Length - 2; i >= 0; i--)
                _lastFps[i + 1] = _lastFps[i];
            // store fps value
            _lastFps[0] = 1000f / timeThisFrame;
        }
    }
}
