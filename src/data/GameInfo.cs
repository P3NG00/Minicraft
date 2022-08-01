using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data
{
    public static class GameInfo
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
}
