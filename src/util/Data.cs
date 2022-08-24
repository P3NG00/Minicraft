using System.IO;
using Minicraft.Game.Blocks;
using Minicraft.Game.Entities;
using Minicraft.Game.Inventories;
using Minicraft.Game.Worlds;

namespace Minicraft.Utils
{
    public static class Data
    {
        public const string SAVE_FILE = "save";

        public static bool SaveExists => File.Exists(SAVE_FILE);

        public static void Save(World world, Inventory inventory, PlayerEntity player)
        {
            using (var stream = new StreamWriter(File.Open(SAVE_FILE, FileMode.Truncate)))
            {
                // write each block
                foreach (var v in world.RawBlockGrid)
                    stream.Write((char)v);
                // TODO write inventory
                // TODO write player position
                // TODO write player health
            }
        }

        public static GameData Load()
        {
            var world = new World();
            using (var stream = new StreamReader(File.Open(SAVE_FILE, FileMode.Open)))
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
                // TODO read inventory
                // TODO read player position
                // TODO read player health
            }
            // TODO return read information instead of new instances
            var inventory = new Inventory();
            var player = new PlayerEntity(world);
            return new GameData(world, inventory, player);
        }
    }
}
