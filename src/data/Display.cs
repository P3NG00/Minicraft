using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Data
{
    public static class Display
    {
        public static SpriteBatch SpriteBatch;
        public static Texture2D TextureSquare;
        public static SpriteFont Font;

        public const int FRAMES_PER_SECOND = 60;
        public const int BLOCK_SCALE_MIN = 5;
        public const int BLOCK_SCALE_MAX = 25;

        public static Point WindowSize = new Point(1280, 720);
        public static readonly float FrameStep = 1f / FRAMES_PER_SECOND;

        public static Vector2 CameraOffset;
        public static bool ShowGrid = false;
        public static int BlockScale = 20;

        public static void Update(Entity player)
        {
            var cameraOffset = -(WindowSize.ToVector2() / 2f);
            cameraOffset.X = cameraOffset.X + (player.Position.X * BlockScale);
            cameraOffset.Y = cameraOffset.Y - ((player.Position.Y + (player.Dimensions.Y / 2f)) * BlockScale);
            CameraOffset = cameraOffset;
        }

        public static void Draw(Vector2 position, Vector2 size, Color color) => SpriteBatch.Draw(TextureSquare, position, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);

        public static void DrawString(Vector2 position, string text, Color color) => SpriteBatch.DrawString(Font, text, position, color);
    }
}
