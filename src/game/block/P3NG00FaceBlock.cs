using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minicraft.Game.Blocks
{
    public sealed class P3NG00FaceBlock : Block
    {
        public P3NG00FaceBlock(string name, Color color, int hitsToBreak, bool canWalkThrough, Texture2D texture) : base(name, color, hitsToBreak, canWalkThrough, texture) {}
    }
}
