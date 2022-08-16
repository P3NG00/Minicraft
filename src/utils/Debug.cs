using System.Collections;
using Microsoft.Xna.Framework;

namespace Minicraft.Utils
{
    public static class Debug
    {
        public const float TIME_SCALE_STEP = 0.05f;

        public static bool Enabled = false;
        public static bool TrackUpdated = false;
        public static float TimeScale
        {
            get => _timeScale;
            set => _timeScale = value.Clamp(0f, 1f);
        }

        private static readonly Hashtable _debugUpdates = new Hashtable();
        private static float _timeScale = 1f;

        public static void AddBlockUpdate(Point blockPos)
        {
            if (_debugUpdates.Contains(blockPos))
                _debugUpdates.Remove(blockPos);
            _debugUpdates.Add(blockPos, Colors.DebugReason_BlockUpdate);
        }

        public static void AddCollisionCheck(Point blockPos)
        {
            if (!_debugUpdates.Contains(blockPos))
                _debugUpdates.Add(blockPos, Colors.DebugReason_CollisionCheck);
        }

        public static void AddAirCheck(Point blockPos)
        {
            if (!_debugUpdates.Contains(blockPos))
                _debugUpdates.Add(blockPos, Colors.DebugReason_AirCheck);
        }

        public static Color? CheckDebugColor(Point blockPos)
        {
            Color? color = null;
            if (_debugUpdates.Contains(blockPos))
                color = (Color)_debugUpdates[blockPos];
            return color;
        }

        public static void ClearDebugUpdates() => _debugUpdates.Clear();
    }
}
