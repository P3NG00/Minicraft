using Microsoft.Xna.Framework;

namespace Minicraft.Scenes
{
    public interface IScene
    {
        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);
    }
}
