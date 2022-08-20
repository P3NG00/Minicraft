using System;
using System.Collections.Immutable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Game.Entities;

namespace Minicraft.Utils
{
    public static class Display
    {
        public const int FRAMES_PER_SECOND = 60;
        public const int BLOCK_SCALE_MIN = 5;
        public const int BLOCK_SCALE_MAX = 25;

        private static ImmutableArray<SpriteFont> _typeWriterFont;

        public static SpriteBatch SpriteBatch { get; private set; }
        public static Texture2D TextureSquare { get; private set; }
        public static Point WindowSize => new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        public static readonly float FrameStep = 1f / FRAMES_PER_SECOND;

        public static Vector2 CameraOffset;
        public static bool ShowGrid = false;
        public static int BlockScale = 20;

        private static GraphicsDeviceManager _graphics;
        private static Point _lastWindowSize;

        public static void Constructor(Microsoft.Xna.Framework.Game game) => _graphics = new GraphicsDeviceManager(game);

        public static void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            // create square for drawing
            TextureSquare = new Texture2D(graphicsDevice, 1, 1);
            TextureSquare.SetData(new[] {new Color(255, 255, 255)});
            // load font
            _typeWriterFont = ImmutableArray.Create(
                content.Load<SpriteFont>("type_writer_12"),
                content.Load<SpriteFont>("type_writer_24"),
                content.Load<SpriteFont>("type_writer_36"));
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

        public static SpriteFont GetFont(FontSize fontSize) => _typeWriterFont[(int)fontSize];

        public static void Draw(Vector2 position, Vector2 size, Color color) => SpriteBatch.Draw(TextureSquare, position, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);

        public static void DrawOffset(Vector2 position, Vector2 size, Color color) => Draw(position - CameraOffset, size, color);

        public static void Draw(Rectangle rectangle, Color color) => SpriteBatch.Draw(TextureSquare, rectangle, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

        // draws faded overlay over entire window
        public static void DrawOverlay() => Display.Draw(Vector2.Zero, WindowSize.ToVector2(), Colors.Overlay);

        // (-1f, -1f) = top-left of window.
        // ( 0f,  0f) = center of window.
        // ( 1f,  1f) = bottom-right of window.
        public static void DrawCenteredText(FontSize fontSize, Vector2 relativeScreenPosition, string text, Color color, Action<FontSize, Vector2, string, Color> drawStringFunc)
        {
            var textSize = GetFont(fontSize).MeasureString(text);
            var screenPosition = ((relativeScreenPosition / 2f) + new Vector2(0.5f)) * WindowSize.ToVector2();
            var drawPos = screenPosition - (textSize / 2f);
            drawStringFunc(fontSize, drawPos, text, color);
        }

        public static void DrawStringWithBackground(FontSize fontSize, Vector2 position, string text, Color color)
        {
            // draw text background
            var textSize = GetFont(fontSize).MeasureString(text);
            var uiSpacerVec = new Vector2(Util.UI_SPACER);
            Draw(position - (uiSpacerVec / 2f), textSize + uiSpacerVec, Colors.TextBackground);
            // draw text
            DrawString(fontSize, position, text, color);
        }

        public static void DrawShadowedString(FontSize fontSize, Vector2 position, string text, Color color)
        {
            // draw shadowed text
            DrawString(fontSize, position + new Vector2(2 * ((int)fontSize + 1)), text, Colors.TextShadow);
            // draw regular text
            DrawString(fontSize, position, text, color);
        }

        public static void DrawString(FontSize fontSize, Vector2 position, string text, Color color) => SpriteBatch.DrawString(GetFont(fontSize), text, position, color);
    }
}
