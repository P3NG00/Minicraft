using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinicraftGame.Texture
{
    public static class Textures
    {
        public const int SIZE = 8;
        private const int EDGE = SIZE - 1;

        public static Texture2D Blank { get; private set; }
        public static Texture2D Shaded { get; private set; }
        public static Texture2D ShadedLeaves { get; private set; }
        public static Texture2D Striped { get; private set; }
        public static Texture2D HighlightRing { get; private set; }
        public static Texture2D P3NG00Face { get; private set; }
        public static Texture2D Stick { get; private set; }

        public static void Initialize()
        {
            Blank = CreateTexture(FilledTexture);
            Shaded = CreateTexture(ShadedTexture);
            ShadedLeaves = CreateTexture(ShadedLeavesTexture);
            Striped = CreateTexture(StripedTexture);
            HighlightRing = CreateTexture(RingHighlightTexture);
            P3NG00Face = CreateTexture(P3NG00FaceTexture);
            Stick = CreateTexture(StickTexture);

            // local funcs
            Texture2D CreateTexture(ColorFunc colorFunc)
            {
                var data = new Color[SIZE * SIZE];
                for (int y = 0; y < SIZE; y++)
                    for (int x = 0; x < SIZE; x++)
                        data[(y * SIZE) + x] = colorFunc(x, y);
                var texture = new Texture2D(Minicraft.Instance.GraphicsDevice, SIZE, SIZE);
                texture.SetData(data);
                return texture;
            }

            Color FilledTexture(int x, int y) => new Color(255, 255, 255);

            Color ShadedTexture(int x, int y)
            {
                var isRightEdge = x == EDGE;
                var isBottomEdge = y == EDGE;
                return isRightEdge || isBottomEdge ? new Color(192, 192, 192) : new Color(255, 255, 255);
            }

            Color ShadedLeavesTexture(int x, int y)
            {
                var transparentPixel = (x + y) % 4 == 0;
                if (transparentPixel)
                    return new Color(0, 0, 0, 0);
                return ShadedTexture(x, y);
            }

            Color StripedTexture(int x, int y)
            {
                var isXStripe = x == 2 || x == 5;
                var isYStripe = y == 2 || y == 5;
                return isXStripe || isYStripe ? new Color(64, 64, 64) : new Color(255, 255, 255);
            }

            Color RingHighlightTexture(int x, int y)
            {
                var isEdgeX = x == 0 || x == EDGE;
                var isEdgeY = y == 0 || y == EDGE;
                return isEdgeX || isEdgeY ? new Color(255, 255, 255) : new Color(0, 0, 0, 0);
            }

            Color P3NG00FaceTexture(int x, int y)
            {
                // 8x8 p3 face texture by color id
                var colorIDs = new[,] {
                    { 0, 1, 1, 1, 1, 1, 1, 0 },
                    { 0, 0, 2, 2, 2, 3, 0, 0 },
                    { 0, 2, 2, 3, 3, 3, 3, 0 },
                    { 4, 4, 3, 3, 4, 4, 4, 4 },
                    { 5, 6, 4, 4, 4, 4, 6, 5 },
                    { 5, 6, 5, 7, 8, 5, 6, 5 },
                    { 9, 9, 5, 10, 7, 5, 5, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9 }};
                // color ids for p3 face texture
                var colorArray = new[] {
                    new Color(132, 171, 185),
                    new Color(163, 203, 210),
                    new Color(246, 237, 203),
                    new Color(220, 208, 188),
                    new Color(195, 184, 173),
                    new Color(168, 159, 156),
                    new Color(0, 0, 0),
                    new Color(195, 170, 51),
                    new Color(224, 209, 84),
                    new Color(144, 137, 136),
                    new Color(163, 130, 27)};
                // return color of face pixel position
                return colorArray[colorIDs[y, x]];
            }

            Color StickTexture(int x, int y) => x + y == 7 ? new Color(255, 255, 255) : new Color(0, 0, 0, 0);
        }

        private delegate Color ColorFunc(int x, int y);
    }
}
