using System.Collections.Immutable;
using Microsoft.Xna.Framework;
using Minicraft.Texture;

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
        TNT = 6,
    }

    public partial class Block
    {
        private static readonly ImmutableArray<Block> s_blockArray;

        public static int Amount => s_blockArray.Length;

        static Block()
        {
            s_blockArray = ImmutableArray.Create(new Block[] {
                new Block("Air", new Color(240, 255, 255), 0, true, Textures.Blank),
                new Block("Dirt Block", new Color(96, 48, 0), 3, false, Textures.Shaded),
                new GrassBlock("Grass Block", new Color(32, 160, 16), 3, false, Textures.Shaded),
                new Block("Stone Block", new Color(192, 192, 192), 7, false, Textures.Shaded),
                new Block("Wood Block", new Color(128, 92, 32), 4, true, Textures.Shaded),
                new LeavesBlock("Leaves Block", new Color(48, 128, 32), 2, true, Textures.Shaded),
                new TNTBlock("TNT", new Color(255, 0, 0), 1, true, Textures.Striped),
            });
        }
    }
}
