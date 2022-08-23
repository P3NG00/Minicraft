using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Minicraft.Utils
{
    public static class Debug
    {
        public const float TIME_SCALE_STEP = 0.05f;

        public static bool Enabled = false;
        public static bool DisplayBlockChecks = false;
        public static float TimeScale
        {
            get => _timeScale;
            set => _timeScale = value.Clamp(0f, 1f);
        }

        private static readonly Hashtable _debugUpdates = new Hashtable();
        private static float _timeScale = 1f;

        public static void Update() => _debugUpdates.Clear();

        public static void AddBlockUpdate(Point blockPos) => Add(blockPos, Colors.DebugReason_BlockUpdate);

        public static void AddCollisionCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_CollisionCheck);

        public static void AddAirCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_AirCheck);

        public static bool HasDebugUpdate(Point blockPos) => _debugUpdates.Contains(blockPos);

        public static Color[] GetDebugColors(Point blockPos) => ((List<Color>)_debugUpdates[blockPos]).ToArray();

        private static void Add(Point blockPos, Color color)
        {
            if (!HasDebugUpdate(blockPos))
                _debugUpdates.Add(blockPos, new List<Color>(new[] {color}));
            else
                ((List<Color>)_debugUpdates[blockPos]).Add(color);
        }
    }
}
