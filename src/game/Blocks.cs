using Microsoft.Xna.Framework;

namespace Minicraft.Game
{
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