using System.IO;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.Worlds;

namespace MinicraftGame.Utils
{
    public static class Data
    {
        public const string SAVE_FILE = "save";

        public static bool SaveExists => File.Exists(SAVE_FILE);

        // TODO save all types of entities: items, npcs, tnt, etc.
        // TODO save cursor slot

        public static void Save()
        {
            using (var stream = new BinaryWriter(File.Open(SAVE_FILE, FileMode.Create)))
            {
                WriteWorldBlocks();
                WriteInventorySlots();
                WritePlayerData();

                void WriteWorldBlocks() => Minicraft.World.RawBlockGrid.ForEach(stream.Write);

                void WriteInventorySlots()
                {
                    for (int i = 0; i < Inventory.SLOTS; i++)
                    {
                        var slot = Minicraft.Player.Inventory[i];
                        stream.Write(slot.Item);
                        stream.Write(slot.Amount);
                    }
                }

                void WritePlayerData()
                {
                    // write player position
                    stream.Write(Minicraft.Player.Position.X);
                    stream.Write(Minicraft.Player.Position.Y);
                    // write player health
                    stream.Write(Minicraft.Player.Life);
                    // write player velocity
                    stream.Write(Minicraft.Player.RawVelocity.X);
                    stream.Write(Minicraft.Player.RawVelocity.Y);
                    // write last grounded height
                    stream.Write(Minicraft.Player.LastGroundedHeight);
                }
            }
        }

        public static void Load()
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
                        var amount = stream.ReadInt32();
                        inventory[i].Set(item, amount);
                    }
                }

                void ReadPlayerData()
                {
                    // read player position
                    var posX = stream.ReadSingle();
                    var posY = stream.ReadSingle();
                    player = new PlayerEntity(new Vector2(posX, posY), inventory);
                    // read player health
                    var life = stream.ReadSingle();
                    player.SetLife(life);
                    // read player velocity
                    var velX = stream.ReadSingle();
                    var velY = stream.ReadSingle();
                    player.RawVelocity = new Vector2(velX, velY);
                    // read last grounded height
                    var lastGroundHeight = stream.ReadSingle();
                    player.LastGroundedHeight = lastGroundHeight;
                }
            }

            Minicraft.Player = player;
            Minicraft.World = world;
        }
    }
}
