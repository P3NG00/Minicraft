using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinicraftGame.Texture;

namespace MinicraftGame.Utils
{
    public struct DrawData
    {
        public readonly Texture2D Texture;
        public readonly Color Color;

        public DrawData(Texture2D texture = null, Color? color = null)
        {
            Texture = texture ?? Textures.Blank;
            Color = color ?? Colors.Default;
        }
    }
}
