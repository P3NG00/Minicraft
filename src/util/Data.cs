using System.IO;
using Microsoft.Xna.Framework;
using Minicraft.Game.BlockType;
using Minicraft.Game.Entities.Living;
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
            using (var stream = new BinaryWriter(File.Open(SAVE_FILE, FileMode.Create)))
            {
                // write each block
                foreach (var blockType in world.RawBlockGrid)
                    stream.WriteBlockType(blockType);
                // write inventory
                for (int i = 0; i < Inventory.SLOTS; i++)
                {
                    var slot = inventory[i];
                    stream.WriteBlockType(slot.Block);
                    stream.Write((char)slot.Amount);
                }
                // write player position
                stream.Write(player.Position.X);
                stream.Write(player.Position.Y);
                // write player health
                stream.Write(player.Life);
            }
        }

        public static GameData Load()
        {
            var world = new World();
            var inventory = new Inventory();
            PlayerEntity player;
            using (var stream = new BinaryReader(File.Open(SAVE_FILE, FileMode.Open)))
            {
                // read each block
                for (int y = 0; y < World.HEIGHT; y++)
                {
                    for (int x = 0; x < World.WIDTH; x++)
                    {
                        var blockType = stream.ReadBlockType();
                        world.SetBlock(x, y, blockType);
                    }
                }
                // read inventory
                for (int i = 0; i < Inventory.SLOTS; i++)
                {
                    var blockType = stream.ReadBlockType();
                    var amount = stream.Read();
                    inventory[i].Set(blockType, amount);
                }
                // read player position
                var posX = stream.ReadSingle();
                var posY = stream.ReadSingle();
                player = new PlayerEntity(new Vector2(posX, posY));
                // read player health
                var life = stream.ReadSingle();
                player.SetLife(life);
            }
            return new GameData(world, inventory, player);
        }

        private static Blocks ReadBlockType(this BinaryReader stream) => (Blocks)stream.Read();

        private static void WriteBlockType(this BinaryWriter stream, Blocks blockType) => stream.Write((char)blockType);
    }
}
