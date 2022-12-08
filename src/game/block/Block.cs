using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.BlockType
{
    public class Block : GameObject
    {
        public readonly int HitsToBreak;
        public readonly bool CanWalkThrough;

        public Block(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id) : base(name, drawData, id)
        {
            HitsToBreak = hitsToBreak;
            CanWalkThrough = canWalkThrough;
        }

        public virtual void RandomTick(Point position) => Debug.AddRandomBlockTick(position);

        public virtual void Interact(Point position) => Debug.AddBlockInteract(position);
    }
}
