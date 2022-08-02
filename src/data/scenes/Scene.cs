using Microsoft.Xna.Framework;

namespace Game.Data.Scenes
{
    public interface Scene
    {
        public void Update(GameTime gameTime);

        public void Draw(GameTime gameTime);
    }
}
