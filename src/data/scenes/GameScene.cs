using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data.Scenes
{
    public sealed class GameScene : Scene
    {
        private readonly Player _player;
        private readonly List<NPC> _npcList = new List<NPC>();
        private readonly World _world;

        public GameScene(World world)
        {
            _world = world;
            _player = new Player(_world);
        }

        public void Update(GameTime gameTime)
        {
            GameInfo.Update(_player, _world, (float)gameTime.ElapsedGameTime.TotalSeconds);
            // handle input
            if (Input.KeyFirstDown(Keys.Tab))
                Display.ShowGrid = !Display.ShowGrid;
            if (Debug.Enabled && Input.KeyFirstDown(Keys.F11))
                Debug.TrackUpdated = !Debug.TrackUpdated;
            if (Input.KeyFirstDown(Keys.F12))
                Debug.Enabled = !Debug.Enabled;
            if (Input.KeyFirstDown(Keys.D1))
                GameInfo.CurrentBlock = Blocks.Dirt;
            if (Input.KeyFirstDown(Keys.D2))
                GameInfo.CurrentBlock = Blocks.Grass;
            if (Input.KeyFirstDown(Keys.D3))
                GameInfo.CurrentBlock = Blocks.Stone;
            if (Input.KeyFirstDown(Keys.D4))
                GameInfo.CurrentBlock = Blocks.Wood;
            if (Input.KeyFirstDown(Keys.D5))
                GameInfo.CurrentBlock = Blocks.Leaves;
            Display.BlockScale = Math.Clamp(Display.BlockScale + Input.ScrollWheel, Display.BLOCK_SCALE_MIN, Display.BLOCK_SCALE_MAX);
            // catch out of bounds
            if (GameInfo.LastMouseBlockInt.X >= 0 && GameInfo.LastMouseBlockInt.X < _world.Width &&
                GameInfo.LastMouseBlockInt.Y >= 0 && GameInfo.LastMouseBlockInt.Y < _world.Height)
            {
                bool ctrl = Input.KeyHeld(Keys.LeftControl) || Input.KeyHeld(Keys.RightControl);
                if (ctrl ? Input.ButtonLeftFirstDown() : Input.ButtonLeftDown())
                    _world.Block(GameInfo.LastMouseBlockInt) = Blocks.Air;
                if (ctrl ? Input.ButtonRightFirstDown() : Input.ButtonRightDown())
                    _world.Block(GameInfo.LastMouseBlockInt) = GameInfo.CurrentBlock;
                if (Input.ButtonMiddleFirstDown())
                    _npcList.Add(new NPC(GameInfo.LastMouseBlock));
            }
            // update for every tick step
            while (GameInfo.Tick())
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
            GameInfo.UpdateFramesPerSecond((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            // update display handler
            Display.Update(_player);
            // draw world
            _world.Draw(_player);
            // draw player
            _player.Draw();
            // draw npc's
            _npcList.ForEach(npc => npc.Draw());
            // draw ui
            UI.Draw(_player, _world);
        }
    }

    static class GameInfo
    {
        private static float _tickDelta = 0f;
        private static int[] _ticks = new [] {0, 0};
        private static int[] _lastTickDifferences = new int[10];
        private static float[] _lastFps = new float[10];

        public static Block CurrentBlock = Blocks.Dirt;
        public static Vector2 LastMouseBlock { get; private set; }
        public static Point LastMouseBlockInt { get; private set; }

        public static int Ticks => _ticks[0];
        public static float AverageFramesPerSecond => _lastFps.Average();
        public static float AverageTicksPerFrame => (float)_lastTickDifferences.Average();

        public static bool Tick()
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
            else
                return false;
        }

        public static void Update(Player player, World world, float timeThisUpdate)
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
            // get block position from mouse
            var mousePos = Input.MousePosition.ToVector2();
            mousePos.Y = Display.WindowSize.Y - mousePos.Y - 1;
            LastMouseBlock = ((mousePos - (Display.WindowSize.ToVector2() / 2f)) / Display.BlockScale) + (player.Position + new Vector2(0, player.Dimensions.Y / 2f));
            LastMouseBlockInt = LastMouseBlock.ToPoint();
        }

        public static void UpdateFramesPerSecond(float timeThisFrame)
        {
            // move values down
            for (int i = _lastFps.Length - 2; i >= 0; i--)
                _lastFps[i + 1] = _lastFps[i];
            // store fps value
            _lastFps[0] = 1000f / timeThisFrame;
        }
    }

    static class UI
    {
        private const int UI_SPACER = 5;

        private static readonly Vector2 BarSize = new Vector2(150, 30);

        public static void Draw(Player player, World world)
        {
            // draw health bar
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (BarSize.X / 2f), Display.WindowSize.Y - BarSize.Y);
            Display.Draw(drawPos, BarSize, Colors.UI_Bar);
            // adjust size to fit within bar
            drawPos += new Vector2(UI_SPACER);
            var healthSize = BarSize - (new Vector2(UI_SPACER) * 2);
            // readjust size to display real health
            healthSize.X *= player.Life / player.MaxLife;
            Display.Draw(drawPos, healthSize, Colors.UI_Life);
            // draw health numbers on top of bar
            var healthString = $"{player.Life:0.#}/{player.MaxLife:0.#}";
            var textSize = Display.Font.MeasureString(healthString);
            drawPos = new Vector2((Display.WindowSize.X / 2f) - (textSize.X / 2f), Display.WindowSize.Y - 22);
            Display.DrawString(drawPos, healthString, Colors.UI_TextLife);
            // draw currently selected block
            drawPos = new Vector2(UI_SPACER, Display.WindowSize.Y - Display.Font.LineSpacing - UI_SPACER);
            Display.DrawString(drawPos, $"current block: {GameInfo.CurrentBlock.Name}", Colors.UI_TextBlock);
            // draw debug
            if (Debug.Enabled)
            {
                drawPos = new Vector2(UI_SPACER);
                foreach (var debugInfo in new[] {
                    $"window_size: {Display.WindowSize.X}x{Display.WindowSize.Y}",
                    $"world_size: {world.Width}x{world.Height}",
                    $"show_grid: {Display.ShowGrid}",
                    $"time: {(GameInfo.Ticks / (float)World.TICKS_PER_SECOND):0.000}",
                    $"ticks: {GameInfo.Ticks} ({World.TICKS_PER_SECOND} ticks/sec)",
                    $"frames_per_second: {GameInfo.AverageFramesPerSecond:0.000}",
                    $"ticks_per_frame: {GameInfo.AverageTicksPerFrame:0.000}",
                    $"x: {player.Position.X:0.000}",
                    $"y: {player.Position.Y:0.000}",
                    $"block_scale: {Display.BlockScale}",
                    $"mouse_x: {GameInfo.LastMouseBlock.X:0.000} ({GameInfo.LastMouseBlockInt.X})",
                    $"mouse_y: {GameInfo.LastMouseBlock.Y:0.000} ({GameInfo.LastMouseBlockInt.Y})",
                    $"player_velocity: {player.Velocity.Length() * player.MoveSpeed:0.000}",
                    $"player_grounded: {player.IsGrounded}"})
                {
                    Display.DrawString(drawPos, debugInfo, Colors.UI_TextDebug);
                    drawPos.Y += UI_SPACER + Display.Font.LineSpacing;
                }
            }
        }
    }
}
