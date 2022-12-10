using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MinicraftGame.Utils
{
    public static class Debug
    {
        public const double TIME_SCALE_STEP = 0.1f;

        public static bool Enabled = false;
        public static bool DisplayBlockChecks = false;
        public static bool GiveBlocksElseItems = true;
        public static double TimeScale
        {
            get => _timeScale;
            set => _timeScale = Math.Clamp(value, 0, 5);
        }

        private static readonly Dictionary<Point, List<Color>> _debugUpdates = new();
        private static double _timeScale = 1f;

        public static void Tick() => _debugUpdates.Clear();

        public static void AddBlockInteract(Point blockPos) => Add(blockPos, Colors.DebugReason_BlockInteract);

        public static void AddRandomBlockTick(Point blockPos) => Add(blockPos, Colors.DebugReason_RandomBlockTick);

        public static void AddCollisionCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_CollisionCheck);

        public static void AddAirCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_AirCheck);

        public static void AddGrassSpreadCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_GrassSpreadCheck);

        public static void AddWoodCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_WoodCheck);

        public static void AddTNTIgnite(Point blockPos) => Add(blockPos, Colors.DebugReason_TNTIgnite);

        public static bool HasDebugUpdate(Point blockPos) => _debugUpdates.ContainsKey(blockPos);

        public static List<Color> GetDebugColors(Point blockPos) => _debugUpdates[blockPos];

        private static void Add(Point blockPos, Color color)
        {
            if (!Enabled || !DisplayBlockChecks)
                return;
            if (HasDebugUpdate(blockPos))
            {
                GetDebugColors(blockPos).Add(color);
                return;
            }
            _debugUpdates.Add(blockPos, new List<Color>(new[] { color }));
        }
    }
}
