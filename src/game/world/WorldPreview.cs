using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Utils;

namespace Minicraft.Game.Worlds
{
    public sealed class WorldPreview
    {
        private readonly Texture2D _worldTexture;
        private readonly Vector2 _relativeScreenPosition;

        public WorldPreview(World world, Vector2 relativeScreenPosition)
        {
            _relativeScreenPosition = relativeScreenPosition;
            _worldTexture = new Texture2D(MinicraftGame.GraphicsDevice, World.WIDTH, World.HEIGHT);
            var data = new Color[World.WIDTH * World.HEIGHT];
            for (int y = 0; y < World.HEIGHT; y++)
            {
                var flipY = World.HEIGHT - y - 1;
                for (int x = 0; x < World.WIDTH; x++)
                {
                    var blockType = world.GetBlockType(x, y);
                    var index = (flipY * World.WIDTH) + x;
                    data[index] = blockType.GetBlock().Color;
                }
            }
            _worldTexture.SetData(data);
        }

        public void Draw() => Display.DrawCentered(_relativeScreenPosition, new Vector2(World.WIDTH, World.HEIGHT), Color.White, _worldTexture);
    }
}
