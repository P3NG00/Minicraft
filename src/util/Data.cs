using System.IO;
using Microsoft.Xna.Framework;
using Minicraft.Game.Entities.Living;
using Minicraft.Game.Inventories;
using Minicraft.Game.Worlds;

namespace Minicraft.Utils
{
    public static class Data
    {
        public const string SAVE_FILE = "save";

        public static bool SaveExists => File.Exists(SAVE_FILE);

        // TODO account for blockitems when loading/saving

        public static void Save(GameData gameData)
        {
            using (var stream = new BinaryWriter(File.Open(SAVE_FILE, FileMode.Create)))
            {
                WriteWorldBlocks();
                WriteInventorySlots();
                WritePlayerData();

                void WriteWorldBlocks()
                {
                    foreach (var block in gameData.World.RawBlockGrid)
                        stream.WriteBlock(block);
                }

                void WriteInventorySlots()
                {
                    for (int i = 0; i < Inventory.SLOTS; i++)
                    {
                        var slot = gameData.Inventory[i];
                        stream.WriteItem(slot.Item);
                        stream.Write((char)slot.Amount);
                    }
                }

                void WritePlayerData()
                {
                    // write player position
                    stream.Write(gameData.Player.Position.X);
                    stream.Write(gameData.Player.Position.Y);
                    // write player health
                    stream.Write(gameData.Player.Life);
                }
            }
        }

        public static GameData Load()
        {
            var world = new World();
            var inventory = new Inventory();
            PlayerEntity player;
            using (var stream = new BinaryReader(File.Open(SAVE_FILE, FileMode.Open)))
            {
                ReadWorldBlocks();
                ReadInventorySlots();
                ReadPlayerData();

                void ReadWorldBlocks()
                {
                    for (int y = 0; y < World.HEIGHT; y++)
                    {
                        for (int x = 0; x < World.WIDTH; x++)
                        {
                            var block = stream.ReadBlock();
                            world.SetBlock(x, y, block);
                        }
                    }
                }

                void ReadInventorySlots()
                {
                    for (int i = 0; i < Inventory.SLOTS; i++)
                    {
                        var item = stream.ReadItem();
                        var amount = stream.Read();
                        inventory[i].Set(item, amount);
                    }
                }

                void ReadPlayerData()
                {
                    // read player position
                    var posX = stream.ReadSingle();
                    var posY = stream.ReadSingle();
                    player = new PlayerEntity(new Vector2(posX, posY));
                    // read player health
                    var life = stream.ReadSingle();
                    player.SetLife(life);
                }
            }
            return new GameData(world, inventory, player);
        }
    }
}
