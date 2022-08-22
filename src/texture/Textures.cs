using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minicraft.Texture
{
    public static class Textures
    {
        public const int SIZE = 8;

        public static Texture2D Blank { get; private set; }
        public static Texture2D Shaded_BottomRight { get; private set; }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            Texture2D CreateTexture(int size = SIZE) => new Texture2D(graphicsDevice, size, size);

            Blank = CreateTexture(1);
            Blank.SetData(new[] {new Color(255, 255, 255)});

            Shaded_BottomRight = CreateTexture();
            var data = new Color[SIZE * SIZE];
            var unshaded = new Color(255, 255, 255);
            var shaded = new Color(192, 192, 192);
            for (var y = 0; y < SIZE; y++)
            {
                for (var x = 0; x < SIZE; x++)
                {
                    bool edgeX = x == SIZE - 1;
                    bool edgeY = y == SIZE - 1;
                    Color color = edgeX || edgeY ? shaded : unshaded;
                    var index = (y * SIZE) + x;
                    data[index] = color;
                }
            }
            Shaded_BottomRight.SetData(data);
        }
    }
}
