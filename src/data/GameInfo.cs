using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data
{
    public static class GameInfo
    {
        private static float _tickDelta = 0f;
        private static Vector2 _mouseBlock;
        private static Point _mouseBlockInt;
        private static int[] _ticks = new [] {0, 0};
        private static int[] _lastTickDifferences = new int[10];
        private static float[] _lastFps = new float[10];

        public static Block CurrentBlock = Blocks.Dirt;

        public static int Ticks => _ticks[0];
        public static Vector2 LastMouseBlock => _mouseBlock;
        public static Point LastMouseBlockInt => _mouseBlockInt;
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
            _mouseBlock = ((mousePos - (Display.WindowSize.ToVector2() / 2f)) / Display.BlockScale) + (player.Position + new Vector2(0, player.Dimensions.Y / 2f));
            _mouseBlockInt = _mouseBlock.ToPoint();
            // catch out of bounds
            if (_mouseBlockInt.X >= 0 && _mouseBlockInt.X < world.Width &&
                _mouseBlockInt.Y >= 0 && _mouseBlockInt.Y < world.Height)
            {
                bool ctrl = Input.KeyHeld(Keys.LeftControl) || Input.KeyHeld(Keys.RightControl);
                if (ctrl ? Input.ButtonLeftFirstDown() : Input.ButtonLeftDown())
                    world.Block(_mouseBlockInt) = Blocks.Air;
                else if (ctrl ? Input.ButtonRightFirstDown() : Input.ButtonRightDown())
                    world.Block(_mouseBlockInt) = CurrentBlock;
                else if (ctrl ? Input.ButtonMiddleFirstDown() : Input.ButtonMiddleDown())
                    world.Block(_mouseBlockInt).Update(_mouseBlockInt, world);
            }
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
}
