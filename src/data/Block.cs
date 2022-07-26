using Microsoft.Xna.Framework;

namespace Game.Data
{
    public class Block
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public bool CanWalkThrough { get; private set; }

        public Block(string name, Color color, bool canWalkThrough = false)
        {
            Name = name;
            Color = color;
            CanWalkThrough = canWalkThrough;
        }

        public virtual void Update(Point position, World world) {}
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
            if (position.Y + 1 == world.Height || world.Block(position + new Point(0, 1)).CanWalkThrough)
            {
                // check blocks to spread to
                var offset = _spreadOffsets.GetRandom();
                var checkPos = position + offset;
                if (checkPos.X >= 0 && checkPos.X < world.Width &&
                    checkPos.Y >= 0 && checkPos.Y < world.Height)
                {
                    var block = world.Block(checkPos);
                    var upPos = checkPos + new Point(0, 1);
                    if (block == Blocks.Dirt && (upPos.Y == world.Height || world.Block(upPos).CanWalkThrough))
                        world.Block(checkPos) = Blocks.Grass;
                }
            }
            else
                world.Block(position) = Blocks.Dirt;
        }
    }

    public sealed class BlockLeaves : Block
    {
        private static readonly Point[] _checkOffsets = new []
        {
            new Point(-1,  1), new Point(0,  1), new Point(1,  1),
            new Point(-1,  0),                   new Point(1,  0),
            new Point(-1, -1), new Point(0, -1), new Point(1, -1)
        };

        public BlockLeaves(string name, Color color, bool canWalkThrough = false) : base (name, color, canWalkThrough) {}

        public sealed override void Update(Point position, World world)
        {
            // check surrounding blocks for logs
            foreach (var offset in _checkOffsets)
                // if wood detected
                if (world.Block(position + offset) == Blocks.Wood)
                    // finish block update
                    return;
            // if reached then no wood was found. remove leaves
            world.Block(position) = Blocks.Air;
        }
    }

    public static class Blocks
    {
        public static readonly Block Air = new Block("Air", new Color(240, 255, 255), true);
        public static readonly Block Dirt = new Block("Dirt", new Color(96, 48, 0));
        public static readonly Block Grass = new BlockGrass("Grass", new Color(48, 160, 32));
        public static readonly Block Stone = new Block("Stone", new Color(192, 192, 192));
        public static readonly Block Wood = new Block("Wood", new Color(128, 92, 32), true);
        public static readonly Block Leaves = new BlockLeaves("Leaves", new Color(64, 224, 48), true);
    }
}
