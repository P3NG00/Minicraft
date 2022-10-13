using System;
using Microsoft.Xna.Framework;
using Minicraft.Texture;

namespace Minicraft.Game.BlockType
{
    public static class Blocks
    {
        public static Block Air { get; private set; }
        public static Block Dirt { get; private set; }
        public static Block Grass { get; private set; }
        public static Block Stone { get; private set; }
        public static Block Wood { get; private set; }
        public static Block Leaves { get; private set; }
        public static Block TNT { get; private set; }
        public static Block P3NG00Face { get; private set; }

        private static Block[] _blocks;

        public static void Initialize()
        {
            // instantiate blocks
            Air = new Block("Air", 0, true, new(color: new Color(240, 255, 255)));
            Dirt = new Block("Dirt Block", 3, false, new(Textures.Shaded, new Color(96, 48, 0)));
            Grass = new GrassBlock("Grass Block", 3, false, new(Textures.Shaded, new Color(32, 160, 16)));
            Stone = new Block("Stone Block", 7, false, new(Textures.Shaded, new Color(192, 192, 192)));
            Wood = new Block("Wood Block", 4, true, new (Textures.Shaded, new Color(128, 92, 32)));
            Leaves = new LeavesBlock("Leaves Block", 2, true, new(Textures.Shaded, new Color(48, 128, 32)));
            TNT = new TNTBlock("TNT", 1, true, new(Textures.Striped, new Color(255, 0, 0)));
            P3NG00Face = new P3NG00FaceBlock("P3NG00's Face as a Block", 1, false, new(Textures.P3NG00Face));
            // add to array in order of id
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