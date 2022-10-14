using System.Collections.Immutable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MinicraftGame.Font
{
    public static class Fonts
    {
        private static ImmutableArray<SpriteFont> s_typeWriterFont;

        public static void Initialize(ContentManager content)
        {
            // load font
            s_typeWriterFont = ImmutableArray.Create(
                Load("type_writer_12"),
                Load("type_writer_24"),
                Load("type_writer_36"));

            // local func
            SpriteFont Load(string name) => content.Load<SpriteFont>(name);
        }

        public static SpriteFont GetFont(this FontSize fontSize) => s_typeWriterFont[(int)fontSize];

        public static Vector2 MeasureString(this FontSize fontSize, string text) => GetFont(fontSize).MeasureString(text);
    }
}
