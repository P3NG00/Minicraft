using System.IO;
using Minicraft.Game.Blocks;
using Minicraft.Game.Worlds;

namespace Minicraft.Utils
{
    public static class Data
    {
        public const string SAVE_FILE = "save";
        // TODO handle saving and loading all game data (inventory, player position, player health)

        public static bool SaveExists => File.Exists(SAVE_FILE);

        public static void SaveWorld(World world)
        {
            using (var stream = new StreamWriter(File.OpenWrite(SAVE_FILE)))
            {
                // write each block
                foreach (var v in world.RawBlockGrid)
                    stream.Write((char)v);
            }
        }

        public static World LoadWorld()
        {
            var world = new World();
            using (var stream = File.OpenText(SAVE_FILE))
            {
                // read each block
                for (int y = 0; y < World.HEIGHT; y++)
                {
                    for (int x = 0; x < World.WIDTH; x++)
                    {
                        var blockType = (BlockType)stream.Read();
                        world.SetBlockType(x, y, blockType);
                    }
                }
            }
            return world;
        }
    }
}
