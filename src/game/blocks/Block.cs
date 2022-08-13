using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Blocks
{
    public partial class Block
    {
        public readonly string Name;
        public readonly Color Color;
        public readonly bool CanWalkThrough;

        public Block(string name, Color color, bool canWalkThrough = false)
        {
            Name = name;
            Color = color;
            CanWalkThrough = canWalkThrough;
        }

        public virtual void Update(Point position, World world)
        {
            // add position to updated list
            if (Debug.Enabled && Debug.TrackUpdated)
                Debug.UpdatedPoints.Add(position);
        }

        public override bool Equals(object obj)
        {
            if (obj is BlockType blocks)
                return this == blocks;
            if (obj is Block block)
                return this == block;
            return false;
        }

        public override int GetHashCode() => Name.GetHashCode() + Color.GetHashCode() + CanWalkThrough.GetHashCode();

        public static implicit operator Block(BlockType blockType) => s_blockArray[((int)blockType)];

        public static bool operator ==(Block block, BlockType blockType) => block == (Block)blockType;

        public static bool operator !=(Block block, BlockType blockType) => block != (Block)blockType;
    }
}
