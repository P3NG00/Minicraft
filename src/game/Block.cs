using Microsoft.Xna.Framework;
using Game.Utils;

namespace Game.Game
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

        public static implicit operator Block(BlockType blockType) => _blockArray[((int)blockType)];

        public static bool operator ==(Block block, BlockType blockType) => block == (Block)blockType;

        public static bool operator !=(Block block, BlockType blockType) => block != (Block)blockType;
    }

    public sealed class BlockGrass : Block
    {
        private static readonly Point[] _spreadOffsets = new []
        {
            new Point(-1,  1), new Point(1,  1),
            new Point(-1,  0), new Point(1,  0),
            new Point(-1, -1), new Point(1, -1),
        };

        public BlockGrass(string name, Color color, bool canWalkThrough = false) : base(name, color, canWalkThrough) {}

        public sealed override void Update(Point position, World world)
        {
            // if able to spread
            if (position.Y + 1 == world.Height || world.BlockTypeAt(position + new Point(0, 1)).GetBlock().CanWalkThrough)
            {
                // check random spread position
                var offset = _spreadOffsets.GetRandom();
                var checkPos = position + offset;
                if (checkPos.X >= 0 && checkPos.X < world.Width &&
                    checkPos.Y >= 0 && checkPos.Y < world.Height)
                {
                    ref BlockType block = ref world.BlockTypeAt(checkPos);
                    var upPos = checkPos + new Point(0, 1);
                    if (block == BlockType.Dirt && (upPos.Y == world.Height || world.BlockTypeAt(upPos).GetBlock().CanWalkThrough))
                        block = BlockType.Grass;
                }
            }
            // if unable to spread
            else
                world.BlockTypeAt(position) = BlockType.Dirt;
            // base call
            base.Update(position, world);
        }
    }

    public sealed class BlockLeaves : Block
    {
        private static readonly Point[] _checkOffsets = new []
        {
            new Point(-1,  1), new Point(0,  1), new Point(1,  1),
            new Point(-1,  0), /*   center   */  new Point(1,  0),
            new Point(-1, -1), new Point(0, -1), new Point(1, -1),
        };

        public BlockLeaves(string name, Color color, bool canWalkThrough = false) : base (name, color, canWalkThrough) {}

        public sealed override void Update(Point position, World world)
        {
            // check surrounding blocks for logs
            bool log = false;
            foreach (var offset in _checkOffsets)
            {
                var checkPos = position + offset;
                // test valid position
                if (checkPos.X >= 0 && checkPos.X < world.Width &&
                    checkPos.Y >= 0 && checkPos.Y < world.Height)
                {
                    // if wood detected
                    if (world.BlockTypeAt(checkPos) == BlockType.Wood)
                    {
                        // set log flag
                        log = true;
                        // break check loop
                        break;
                    }
                }
            }
            // if no log, remove leaves
            if (!log)
                world.BlockTypeAt(position) = BlockType.Air;
            // base call
            base.Update(position, world);
        }
    }
}
