using System.Collections.Immutable;
using Microsoft.Xna.Framework;

namespace Minicraft.Game.Blocks
{
    public enum BlockType
    {
        Air = 0,
        Dirt = 1,
        Grass = 2,
        Stone = 3,
        Wood = 4,
        Leaves = 5,
    }

    public partial class Block
    {
        private static readonly ImmutableArray<Block> s_blockArray;

        static Block()
        {
            s_blockArray = ImmutableArray.Create<Block>(new Block[] {
                new Block("Air", new Color(240, 255, 255), true),
                new Block("Dirt Block", new Color(96, 48, 0)),
                new GrassBlock("Grass Block", new Color(48, 160, 32)),
                new Block("Stone Block", new Color(192, 192, 192)),
                new Block("Wood Block", new Color(128, 92, 32), true),
                new LeavesBlock("Leaves Block", new Color(64, 224, 48), true),
            });
        }
    }
}
