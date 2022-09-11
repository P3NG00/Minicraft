using Microsoft.Xna.Framework;

namespace Minicraft.Scenes
{
    public abstract class AbstractScene
    {
        public readonly Color BackgroundColor;

        public AbstractScene(Color backgroundColor) => BackgroundColor = backgroundColor;

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }
}
