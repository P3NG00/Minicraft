using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Font;
using Minicraft.Game.Entities;
using Minicraft.Texture;

namespace Minicraft.Utils
{
    public static class Display
    {
        public const int FRAMES_PER_SECOND = 60;
        public const float FRAME_STEP = 1f / FRAMES_PER_SECOND;
        public const int BLOCK_SCALE_MIN = 5;
        public const int BLOCK_SCALE_MAX = 25;

        public static SpriteBatch SpriteBatch { get; private set; }
        public static Point WindowSize => new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        public static Vector2 CameraOffset;
        public static bool ShowGrid = false;
        public static int BlockScale = 16;

        private static GraphicsDeviceManager _graphics;
        private static Point _lastWindowSize;

        public static void Constructor(Microsoft.Xna.Framework.Game game) => _graphics = new GraphicsDeviceManager(game);

        public static void LoadContent(GraphicsDevice graphicsDevice)
        {
            // create display handler
            SpriteBatch = new SpriteBatch(graphicsDevice);
        }

        public static void Initialize() => SetSize(1280, 720);

        public static void UpdateCameraOffset(PlayerEntity player)
        {
            var cameraOffset = -(WindowSize.ToVector2() / 2f);
            cameraOffset.X = cameraOffset.X + (player.Position.X * BlockScale);
            cameraOffset.Y = cameraOffset.Y - ((player.Position.Y + (player.Dimensions.Y / 2f)) * BlockScale);
            CameraOffset = cameraOffset;
        }

        public static void ToggleFullscreen()
        {
            if (_graphics.IsFullScreen)
                SetSize(_lastWindowSize.X, _lastWindowSize.Y);
            else
            {
                _lastWindowSize = new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                SetSize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, true);
            }
        }

        public static void SetSize(int width, int height, bool fullscreen = false)
        {
            _graphics.IsFullScreen = fullscreen;
            UpdateSize(width, height);
            _graphics.ApplyChanges();
        }

        public static void UpdateSize(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
        }

        public static void Draw(Vector2 position, Vector2 size, Color color, Texture2D texture = null)
        {
            var t = texture ?? Textures.Blank;
            var scale = size / t.Bounds.Size.ToVector2();
            SpriteBatch.Draw(t, position, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public static void DrawOffset(Vector2 position, Vector2 size, Color color, Texture2D texture = null) => Draw(position - CameraOffset, size, color, texture);

        // draws faded overlay over entire window
        public static void DrawOverlay() => Display.Draw(Vector2.Zero, WindowSize.ToVector2(), Colors.Overlay);

        // (0f, 0f) = top-left of window.
        // (1f, 1f) = bottom-right of window.
        public static void DrawCenteredText(FontSize fontSize, Vector2 relativeScreenPosition, string text, Color color, DrawStringFunc drawStringFunc)
        {
            var textSize = fontSize.MeasureString(text);
            var screenPosition = relativeScreenPosition * WindowSize.ToVector2();
            var drawPos = screenPosition - (textSize / 2f);
            drawStringFunc(fontSize, drawPos, text, color);
        }

        public static void DrawStringWithBackground(FontSize fontSize, Vector2 position, string text, Color color)
        {
            // draw text background
            var textSize = fontSize.MeasureString(text);
            var uiSpacerVec = new Vector2(Util.UI_SPACER);
            Draw(position - (uiSpacerVec / 2f), textSize + uiSpacerVec, Colors.TextBackground);
            // draw text
            DrawString(fontSize, position, text, color);
        }

        public static void DrawStringWithShadow(FontSize fontSize, Vector2 position, string text, Color color)
        {
            // draw shadowed text
            DrawString(fontSize, position + new Vector2(2 * ((int)fontSize + 1)), text, Colors.TextShadow);
            // draw regular text
            DrawString(fontSize, position, text, color);
        }

        public static void DrawString(FontSize fontSize, Vector2 position, string text, Color color) => SpriteBatch.DrawString(fontSize.GetFont(), text, position, color);

        public delegate void DrawStringFunc(FontSize fontSize, Vector2 position, string text, Color color);
    }
}
