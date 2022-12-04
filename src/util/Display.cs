using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinicraftGame.Font;
using MinicraftGame.Texture;

namespace MinicraftGame.Utils
{
    public static class Display
    {
        public const int FRAMES_PER_SECOND = 60;
        public const float FRAME_STEP = 1f / FRAMES_PER_SECOND;
        public const int BLOCK_SCALE_MIN = Textures.SIZE;
        public const int BLOCK_SCALE_MAX = Textures.SIZE * 5;

        public static SpriteBatch SpriteBatch { get; private set; }
        public static Point WindowSize => new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        public static Vector2 CameraOffset;
        public static int BlockScale = Textures.SIZE * 2;

        private static GraphicsDeviceManager _graphics;
        private static Point _lastWindowSize;

        public static void CreateGraphicsManager(Microsoft.Xna.Framework.Game game) => _graphics = new GraphicsDeviceManager(game);

        public static void LoadContent() => SpriteBatch = new SpriteBatch(Minicraft.GraphicsDevice);

        public static void Initialize() => SetSize(1280, 720, false);

        public static void UpdateCameraOffset()
        {
            var centeredScreen = -(WindowSize.ToVector2() / 2f);
            var relativePlayerPosition = Minicraft.Player.Center * BlockScale;
            CameraOffset = new Vector2(centeredScreen.X + relativePlayerPosition.X,
                                       centeredScreen.Y - relativePlayerPosition.Y);
        }

        public static void ToggleFullscreen()
        {
            if (_graphics.IsFullScreen)
                SetSize(_lastWindowSize.X, _lastWindowSize.Y, false);
            else
            {
                _lastWindowSize = WindowSize;
                SetSize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, true);
            }
        }

        public static void SetSize(int width, int height, bool fullscreen)
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

        public static void Draw(Vector2 position, Vector2 size, DrawData drawData, float rotation = 0f)
        {
            var textureSize = drawData.Texture.Bounds.Size.ToVector2();
            var scale = size / textureSize;
            var origin = textureSize / 2f;
            var drawOffset = position + (size / 2f);
            SpriteBatch.Draw(drawData.Texture, drawOffset, null, drawData.Color, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public static void DrawCentered(Vector2 relativeScreenPosition, Vector2 size, DrawData drawData)
        {
            var textureSize = drawData.Texture.Bounds.Size.ToVector2();
            var screenPosition = relativeScreenPosition * WindowSize.ToVector2();
            var drawPos = screenPosition - (textureSize / 2f);
            Draw(drawPos, size, drawData);
        }

        public static void DrawOffset(Vector2 position, Vector2 size, DrawData drawData)
        {
            var drawPos = position - CameraOffset;
            Draw(drawPos, size, drawData);
        }

        // draws faded overlay over entire window
        public static void DrawFadedOverlay() => Display.Draw(Vector2.Zero, WindowSize.ToVector2(), new(color: Colors.Overlay));

        public static void DrawString(FontSize fontSize, Vector2 position, string text, Color color, Vector2? scale = null, float rotation = 0f)
        {
            scale ??= Vector2.One;
            var origin = Fonts.GetFont(fontSize).MeasureString(text) / 2f;
            SpriteBatch.DrawString(fontSize.GetFont(), text, position + origin, color, rotation, origin, scale.Value, SpriteEffects.None, 0f);
        }

        public static void DrawStringWithBackground(FontSize fontSize, Vector2 position, string text, Color color, Vector2? scale = null, float rotation = 0f)
        {
            // draw text background
            var textSize = fontSize.MeasureString(text);
            var uiSpacerVec = new Vector2(Util.UI_SPACER);
            Draw(position - (uiSpacerVec / 2f), textSize + uiSpacerVec, new(color: Colors.TextBackground), rotation);
            // draw text
            DrawString(fontSize, position, text, color, scale, rotation);
        }

        public static void DrawStringWithShadow(FontSize fontSize, Vector2 position, string text, Color color, Vector2? scale = null, float rotation = 0f)
        {
            // draw shadowed text
            var shadowOffset = new Vector2(2 * ((int)fontSize + 1));
            DrawString(fontSize, position + shadowOffset, text, Colors.TextShadow, scale, rotation);
            // draw regular text
            DrawString(fontSize, position, text, color, scale, rotation);
        }

        // (0f, 0f) = top-left of window.
        // (1f, 1f) = bottom-right of window.
        public static void DrawCenteredString(FontSize fontSize, Vector2 relativeScreenPosition, string text, Color color, Vector2? scale = null, float rotation = 0f, DrawStringFunc drawStringFunc = null)
        {
            drawStringFunc ??= DrawString;
            var textSize = fontSize.MeasureString(text);
            var screenPosition = relativeScreenPosition * WindowSize.ToVector2();
            var drawPos = screenPosition - (textSize / 2f);
            drawStringFunc(fontSize, drawPos, text, color, scale, rotation);
        }

        public static void DrawOffsetString(FontSize fontSize, Vector2 position, string text, Color color, Vector2? scale = null, float rotation = 0f, DrawStringFunc drawStringFunc = null)
        {
            drawStringFunc ??= DrawString;
            var drawPos = position - CameraOffset;
            drawStringFunc(fontSize, drawPos, text, color, scale, rotation);
        }

        public delegate void DrawStringFunc(FontSize fontSize, Vector2 position, string text, Color color, Vector2? scale, float rotation);
    }
}
