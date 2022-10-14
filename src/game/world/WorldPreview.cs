using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Worlds
{
    public sealed class WorldPreview
    {
        private readonly Texture2D _worldTexture;
        private readonly Vector2 _relativeScreenPosition;

        // TODO this doesn't need to be it's own class. it can be integrated with WorldCreationScene

        public WorldPreview(Vector2 relativeScreenPosition)
        {
            _relativeScreenPosition = relativeScreenPosition;
            _worldTexture = new Texture2D(Minicraft.GraphicsDevice, World.WIDTH, World.HEIGHT);
            var data = new Color[World.WIDTH * World.HEIGHT];
            for (int y = 0; y < World.HEIGHT; y++)
            {
                var flipY = World.HEIGHT - y - 1;
                for (int x = 0; x < World.WIDTH; x++)
                {
                    var block = Minicraft.World.GetBlock(x, y);
                    var index = (flipY * World.WIDTH) + x;
                    data[index] = block.Color;
                }
            }
            _worldTexture.SetData(data);
        }

        public void Draw() => Display.DrawCentered(_relativeScreenPosition, new Vector2(World.WIDTH, World.HEIGHT), new(_worldTexture));
    }
}
