using Microsoft.Xna.Framework;

namespace Minicraft.Scenes
{
    public interface Scene
    {
        public void Update(GameTime gameTime);

        public void Draw(GameTime gameTime);
    }
}
