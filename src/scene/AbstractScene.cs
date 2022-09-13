using Microsoft.Xna.Framework;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public abstract class AbstractScene
    {
        public readonly Color BackgroundColor;

        public AbstractScene(Color? backgroundColor = null) => BackgroundColor = backgroundColor ?? Colors.Background;

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }
}
