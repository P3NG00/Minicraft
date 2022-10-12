using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.BlockType
{
    public partial class Block
    {
        public readonly string Name;
        public readonly Color Color;
        public readonly int HitsToBreak;
        public readonly bool CanWalkThrough;
        public readonly Texture2D Texture;

        public Block(string name, Color color, int hitsToBreak, bool canWalkThrough, Texture2D texture)
        {
            Name = name;
            Color = color;
            HitsToBreak = hitsToBreak;
            CanWalkThrough = canWalkThrough;
            Texture = texture;
        }

        public virtual void Update(World world, Point position) => Debug.AddBlockUpdate(position);

        public virtual void Interact(World world, Point position) => Debug.AddBlockInteract(position);

        public override bool Equals(object obj)
        {
            if (obj is Blocks blocks)
                return this == blocks;
            if (obj is Block block)
                return this == block;
            return false;
        }

        public override int GetHashCode() => Name.GetHashCode() + Color.GetHashCode() + CanWalkThrough.GetHashCode();

        public static implicit operator Block(Blocks blockType) => s_blockArray[((int)blockType)];

        public static bool operator ==(Block block, Blocks blockType) => block == (Block)blockType;

        public static bool operator !=(Block block, Blocks blockType) => block != (Block)blockType;
    }
}
