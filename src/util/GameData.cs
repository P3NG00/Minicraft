using Minicraft.Game.Entities;
using Minicraft.Game.Inventories;
using Minicraft.Game.Worlds;

namespace Minicraft.Utils
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
