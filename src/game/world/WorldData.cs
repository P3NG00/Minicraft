using System.IO;
using Minicraft.Game.Blocks;

namespace Minicraft.Game.Worlds
{
    public sealed partial class World
    {
        public void Save()
        {
            using (var stream = new StreamWriter(File.OpenWrite(World.SAVE_FILE)))
            {
                // write each block
                foreach (var v in _blockGrid)
                    stream.Write((char)v);
            }
        }

        public static World Load()
        {
            var world = new World();
            using (var stream = File.OpenText(World.SAVE_FILE))
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
