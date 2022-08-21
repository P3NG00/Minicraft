using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minicraft.Texture
{
    public static class Textures
    {
        public static Texture2D Blank_1x { get; private set; }
        public static Texture2D Shaded_2x { get; private set; }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            Texture2D CreateTexture(int size) => new Texture2D(graphicsDevice, size, size);

            Blank_1x = CreateTexture(1);
            Blank_1x.SetData(new[] {new Color(255, 255, 255)});

            Shaded_2x = CreateTexture(2);
            Shaded_2x.SetData(new[] {
                new Color(255, 255, 255), new Color(192, 192, 192),
                new Color(192, 192, 192), new Color(128, 128, 128)});
        }
    }
}
