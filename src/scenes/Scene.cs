using Microsoft.Xna.Framework;

namespace Minicraft.Scenes
{
    public interface Scene
    {
        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);
    }
}
