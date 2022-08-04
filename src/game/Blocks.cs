using Microsoft.Xna.Framework;

namespace Minicraft.Game
{
    public enum Blocks
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
        private static readonly Block[] _blockArray = new []
        {
            new Block("Air", new Color(240, 255, 255), true),
            new Block("Dirt", new Color(96, 48, 0)),
            new BlockGrass("Grass", new Color(48, 160, 32)),
            new Block("Stone", new Color(192, 192, 192)),
            new Block("Wood", new Color(128, 92, 32), true),
            new BlockLeaves("Leaves", new Color(64, 224, 48), true),
        };
    }
}
