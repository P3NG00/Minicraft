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
        // TODO TNT
    }

    public partial class Block
    {
        private static readonly ImmutableArray<Block> s_blockArray;

        static Block()
        {
            s_blockArray = ImmutableArray.Create(new Block[] {
                new Block("Air", new Color(240, 255, 255), true),
                new Block("Dirt Block", new Color(96, 48, 0), texture: Textures.Shaded_2x),
                new GrassBlock("Grass Block", new Color(32, 128, 16), texture: Textures.Shaded_2x),
                new Block("Stone Block", new Color(192, 192, 192), texture: Textures.Shaded_2x),
                new Block("Wood Block", new Color(128, 92, 32), true, Textures.Shaded_2x),
                new LeavesBlock("Leaves Block", new Color(48, 128, 32), true, Textures.Shaded_2x),
            });
        }
    }
}
