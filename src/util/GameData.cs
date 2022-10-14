using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.Worlds;

namespace MinicraftGame.Utils
{
    public struct GameData
    {
        public readonly World World;
        public readonly Inventory Inventory;
        public readonly PlayerEntity Player;

        public GameData(World world, Inventory inventory, PlayerEntity player)
        {
            World = world;
            Inventory = inventory;
            Player = player;
        }
    }
}
