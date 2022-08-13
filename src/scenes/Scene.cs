using Microsoft.Xna.Framework;

namespace Game.Scenes
{
    public interface Scene
    {
        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);
    }
}
