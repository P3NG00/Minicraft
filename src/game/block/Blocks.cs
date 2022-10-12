using System;
using Microsoft.Xna.Framework;
using Minicraft.Texture;

namespace Minicraft.Game.BlockType
{
    public static class Blocks
    {
        public static Block Air;
        public static Block Dirt;
        public static Block Grass;
        public static Block Stone;
        public static Block Wood;
        public static Block Leaves;
        public static Block TNT;
        public static Block P3NG00Face;

        private static Block[] _blocks;

        public static void Initialize()
        {
            Air = new Block("Air", 0, true, new(color: new Color(240, 255, 255)));
            Dirt = new Block("Dirt Block", 3, false, new(Textures.Shaded, new Color(96, 48, 0)));
            Grass = new GrassBlock("Grass Block", 3, false, new(Textures.Shaded, new Color(32, 160, 16)));
            Stone = new Block("Stone Block", 7, false, new(Textures.Shaded, new Color(192, 192, 192)));
            Wood = new Block("Wood Block", 4, true, new (Textures.Shaded, new Color(128, 92, 32)));
            Leaves = new LeavesBlock("Leaves Block", 2, true, new(Textures.Shaded, new Color(48, 128, 32)));
            TNT = new TNTBlock("TNT", 1, true, new(Textures.Striped, new Color(255, 0, 0)));
            P3NG00Face = new P3NG00FaceBlock("P3NG00's Face as a Block", 1, false, new(Textures.P3NG00Face));

            _blocks = new[]
            {
                Air,            // 0
                Dirt,           // 1
                Grass,          // 2
                Stone,          // 3
                Wood,           // 4
                Leaves,         // 5
                TNT,            // 6
                P3NG00Face,     // 7
            };
        }

        public static int Amount => _blocks.Length;

        public static Block GetByID(int i) => _blocks[i];

        public static int GetID(Block block) => Array.IndexOf(_blocks, block);
    }
}
