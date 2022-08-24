using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minicraft.Texture
{
    public static class Textures
    {
        public const int SIZE = 8;
        private const int RIGHT_EDGE = SIZE - 1;

        public static Texture2D Blank { get; private set; }
        public static Texture2D HighlightRing { get; private set; }
        public static Texture2D Shaded { get; private set; }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            Blank = CreateTexture(graphicsDevice, CreateFilledTexture);
            Shaded = CreateTexture(graphicsDevice, CreateShadedTexture);
            HighlightRing = CreateTexture(graphicsDevice, CreateRingHighlightTexture);
        }

        private static Texture2D CreateTexture(GraphicsDevice graphicsDevice, ColorFunc colorFunc)
        {
            var data = new Color[SIZE * SIZE];
            for (int y = 0; y < SIZE; y++)
                for (int x = 0; x < SIZE; x++)
                    data[(y * SIZE) + x] = colorFunc(x, y);
            var texture = new Texture2D(graphicsDevice, SIZE, SIZE);
            texture.SetData(data);
            return texture;
        }

        private static Color CreateFilledTexture(int x, int y) => new Color(255, 255, 255);

        private static Color CreateShadedTexture(int x, int y)
        {
            var isRightEdgeX = x == RIGHT_EDGE;
            var isEdgeY = y == RIGHT_EDGE;
            return isRightEdgeX || isEdgeY ? new Color(192, 192, 192) : new Color(255, 255, 255);
        }

        private static Color CreateRingHighlightTexture(int x, int y)
        {
            var isEdge = x == 0 || x == RIGHT_EDGE ||
                         y == 0 || y == RIGHT_EDGE;
            return isEdge ? new Color(255, 255, 255) : new Color(0, 0, 0, 0);
        }

        private delegate Color ColorFunc(int x, int y);
    }
}
