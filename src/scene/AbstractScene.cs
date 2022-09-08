using Microsoft.Xna.Framework;

namespace Minicraft.Scenes
{
    public abstract class Scene
    {
        public readonly Color BackgroundColor;

        public Scene(Color backgroundColor) => BackgroundColor = backgroundColor;

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }
}
