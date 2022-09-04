using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minicraft.Game.Blocks
{
    public sealed class TNTBlock : Block
    {
        public TNTBlock(string name, Color color, int hitsToBreak, bool canWalkThrough, Texture2D texture) : base(name, color, hitsToBreak, canWalkThrough, texture) {}

        // TODO explode on break
    }
}
