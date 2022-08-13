using Microsoft.Xna.Framework;

namespace Game.Scenes
{
    public interface IScene
    {
        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);
    }
}
