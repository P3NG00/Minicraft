using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Data
{
    public sealed class Display
    {
        public static Texture2D TextureSquare;
        public static SpriteFont Font;

        public SpriteBatch SpriteBatch { get; private set; }
        public Point WindowSize { get; private set; }
        public int FramesPerSecond { get; private set; }
        public float FrameStep { get; private set; }
        public int TicksPerSecond { get; private set; }
        public float TickStep { get; private set; }
        public Vector2 CameraOffset { get; private set; }

        public bool ShowGrid = false;
        public int BlockScale;

        public Display(SpriteBatch spriteBatch, Point windowSize, int blockScale, int fps, int tps)
        {
            SpriteBatch = spriteBatch;
            WindowSize = windowSize;
            FramesPerSecond = fps;
            FrameStep = 1f / FramesPerSecond;
            TicksPerSecond = tps;
            TickStep = 1f / TicksPerSecond;
            BlockScale = blockScale;
        }

        public void Update(Entity player)
        {
            var cameraOffset = -(WindowSize.ToVector2() / 2f);
            cameraOffset.X = cameraOffset.X + (player.Position.X * BlockScale);
            cameraOffset.Y = cameraOffset.Y - (player.Position.Y * BlockScale);
            CameraOffset = cameraOffset;
        }

        public void Draw(Vector2 position, Vector2 size, Color color) => SpriteBatch.Draw(TextureSquare, position, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);

        public void DrawString(Vector2 position, string text, Color color) => SpriteBatch.DrawString(Font, text, position, color);
    }
}
