using Microsoft.Xna.Framework;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.BlockType
{
    public sealed class TNTBlock : Block
    {
        public TNTBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1) : base(name, hitsToBreak, canWalkThrough, drawData, id) {}

        public sealed override void Interact(Point position)
        {
            // remove block
            Minicraft.World.SetBlock(position, Blocks.Air);
            // summon TNTEntity
            Minicraft.World.AddEntity(new TNTEntity(position.ToVector2() + new Vector2(0.5f, 0)));
            // base call
            base.Interact(position);
        }
    }
}
