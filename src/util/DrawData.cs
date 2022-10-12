using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Texture;

namespace Minicraft.Utils
{
    public struct DrawData
    {
        public readonly Texture2D Texture;
        public readonly Color Color;

        // TODO cache these if used in classes that are updated frequently
        public DrawData(Texture2D texture = null, Color? color = null)
        {
            Texture = texture ?? Textures.Blank;
            Color = color ?? Colors.Default;
        }
    }
}
