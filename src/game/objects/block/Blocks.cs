using Microsoft.Xna.Framework;
using MinicraftGame.Texture;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects.BlockObject
{
    public sealed class Blocks : ObjectManager<Block>
    {
        private static Blocks _instance;

        public static Block Air { get; private set; }
        public static Block Dirt { get; private set; }
        public static Block Grass { get; private set; }
        public static Block Stone { get; private set; }
        public static Block Wood { get; private set; }
        public static Block Leaves { get; private set; }
        public static Block TNT { get; private set; }
        public static Block P3NG00Face { get; private set; }

        public Blocks() => this.SingletonCheck(ref _instance);

        protected sealed override void InstantiateObjects()
        {
            Air = new Block("Air", 0, true, new(color: new Color(240, 255, 255)), 0);
            Dirt = new Block("Dirt Block", 3, false, new(Textures.Shaded, new Color(96, 48, 0)), 1);
            Grass = new GrassBlock("Grass Block", 3, false, new(Textures.Shaded, new Color(32, 160, 16)), 2);
            Stone = new Block("Stone Block", 7, false, new(Textures.Shaded, new Color(192, 192, 192)), 3);
            Wood = new Block("Wood Block", 4, true, new (Textures.Shaded, new Color(128, 92, 32)), 4);
            Leaves = new LeavesBlock("Leaves Block", 2, true, new(Textures.ShadedLeaves, new Color(48, 128, 32)), 5);
            TNT = new TNTBlock("TNT", 1, true, new(Textures.Striped, new Color(255, 0, 0)), 6);
            P3NG00Face = new P3NG00FaceBlock("P3NG00's Face as a Block", 1, false, new(Textures.P3NG00Face), 7);
        }

        protected sealed override void AddObjects(out Block[] blocks)
        {
            blocks = new[]
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

        public static int Amount => _instance.ObjectAmount;

        public static Block FromID(int i) => _instance.ObjectFromID(i);
    }
}
