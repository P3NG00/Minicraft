using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.BlockType
{
    public class Block
    {
        public readonly string Name;
        public readonly int HitsToBreak;
        public readonly bool CanWalkThrough;
        public readonly DrawData DrawData;

        public Texture2D Texture => DrawData.Texture;
        public Color Color => DrawData.Color;

        public Block(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData)
        {
            Name = name;
            HitsToBreak = hitsToBreak;
            CanWalkThrough = canWalkThrough;
            DrawData = drawData;
        }

        public virtual void Update(World world, Point position) => Debug.AddBlockUpdate(position);

        public virtual void Interact(World world, Point position) => Debug.AddBlockInteract(position);
    }
}
