using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.BlockType
{
    public sealed class TNTBlock : Block
    {
        public TNTBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1) : base(name, hitsToBreak, canWalkThrough, drawData, id) {}

        // TODO summon TNTEntity

        public sealed override void Interact(Point position)
        {
            // play explosion sound
            Audio.Explosion.Play();
            // break blocks in a 5x5 area except for corners
            for (int x = position.X - 2; x <= position.X + 2; x++)
            {
                for (int y = position.Y - 2; y <= position.Y + 2; y++)
                {
                    var isEdgeX = x == position.X - 2 || x == position.X + 2;
                    var isEdgeY = y == position.Y - 2 || y == position.Y + 2;
                    if (!isEdgeX || !isEdgeY)
                        Minicraft.World.SetBlock(x, y, Blocks.Air);
                }
            }
            // base call
            base.Interact(position);
        }
    }
}
