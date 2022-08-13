using System.Collections;
using Microsoft.Xna.Framework;

namespace Minicraft.Utils
{
    public static class Debug
    {
        private static readonly Hashtable _debugUpdates = new Hashtable();

        public static bool Enabled = false;
        public static bool TrackUpdated = false;

        public static void AddBlockUpdate(Point blockPos)
        {
            if (!_debugUpdates.Contains(blockPos))
                _debugUpdates.Add(blockPos, Colors.DebugReason_BlockUpdate);
        }

        public static void AddCollisionCheck(Point blockPos)
        {
            if (_debugUpdates.Contains(blockPos))
                _debugUpdates.Remove(blockPos);
            _debugUpdates.Add(blockPos, Colors.DebugReason_CollisionCheck);
        }

        public static Color? CheckDebugColor(Point blockPos)
        {
            Color? color = null;
            if (_debugUpdates.ContainsKey(blockPos))
            {
                color = (Color)_debugUpdates[blockPos];
                _debugUpdates.Remove(blockPos);
            }
            return color;
        }

        public static void ClearDebugUpdates() => _debugUpdates.Clear();
    }
}
