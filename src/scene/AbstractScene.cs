using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public abstract class AbstractScene
    {
        public readonly Color BackgroundColor;

        public AbstractScene(Color? backgroundColor = null) => BackgroundColor = backgroundColor ?? Colors.Background;

        public virtual void Update() {}

        public virtual void Tick() {}

        public virtual void Draw() {}
    }
}
