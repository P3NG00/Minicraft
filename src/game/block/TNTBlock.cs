using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Game.Worlds;

namespace Minicraft.Game.Blocks
{
    public sealed class TNTBlock : Block
    {
        public TNTBlock(string name, Color color, int hitsToBreak, bool canWalkThrough, Texture2D texture) : base(name, color, hitsToBreak, canWalkThrough, texture) {}

        public sealed override void Interact(World world, Point position)
        {
            // break blocks in a 5x5 area except for corners
            for (int x = position.X - 2; x <= position.X + 2; x++)
            {
                for (int y = position.Y - 2; y <= position.Y + 2; y++)
                {
                    var isEdgeX = x == position.X - 2 || x == position.X + 2;
                    var isEdgeY = y == position.Y - 2 || y == position.Y + 2;
                    if (!isEdgeX || !isEdgeY)
                        world.SetBlockType(x, y, BlockType.Air);
                }
            }
            // base call
            base.Interact(world, position);
        }
    }
}
