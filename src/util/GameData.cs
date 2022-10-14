using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.Worlds;

namespace MinicraftGame.Utils
{
    public struct GameData
    {
        public readonly World World;
        public readonly PlayerEntity Player;

        public GameData(World world, PlayerEntity player)
        {
            World = world;
            Player = player;
        }
    }
}
