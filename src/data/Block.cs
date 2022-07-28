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

        public virtual void Update(Point position, World world)
        {
            // add position to updated list
            if (Debug.Enabled && Debug.TrackUpdated)
                Debug.UpdatedPoints.Add(position);
        }
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
            // if unable to spread
            else
                world.Block(position) = Blocks.Dirt;
            // base call
            base.Update(position, world);
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
            bool log = false;
            foreach (var offset in _checkOffsets)
            {
                // if wood detected
                if (world.Block(position + offset) == Blocks.Wood)
                {
                    // set log flag
                    log = true;
                    // break check loop
                    break;
                }
            }
            // if no log, remove leaves
            if (!log)
                world.Block(position) = Blocks.Air;
            // base call
            base.Update(position, world);
        }
    }
}
