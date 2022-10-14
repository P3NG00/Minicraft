using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.BlockType
{
    public class Block
    {
        public readonly int ID;
        public readonly string Name;
        public readonly int HitsToBreak;
        public readonly bool CanWalkThrough;
        public readonly DrawData DrawData;

        public Texture2D Texture => DrawData.Texture;
        public Color Color => DrawData.Color;

        public Block(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1)
        {
            ID = id;
            Name = name;
            HitsToBreak = hitsToBreak;
            CanWalkThrough = canWalkThrough;
            DrawData = drawData;
        }

        public virtual void RandomTick(Point position) => Debug.AddRandomBlockTick(position);

        public virtual void Interact(Point position) => Debug.AddBlockInteract(position);
    }
}
