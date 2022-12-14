using Microsoft.Xna.Framework;
using MinicraftGame.Input;

namespace MinicraftGame.Utils
{
    public abstract class AbstractHighlightable
    {
        protected Rectangle LastRectangle { get; private set; }
        protected Vector2 RelativeCenter { get; private set; }
        protected bool Highlighted { get; private set; }

        protected virtual Point Size
        {
            get
            {
                if (!_size.HasValue)
                    throw new System.Exception("If size is not specified, override Size property.");
                return _size.Value;
            }
        }

        private readonly Point? _size;

        public AbstractHighlightable(Vector2 relativeCenter, Point? size = null)
        {
            RelativeCenter = relativeCenter;
            _size = size;
        }

        protected abstract Rectangle GetRect();

        public virtual void Update()
        {
            LastRectangle = GetRect();
            Highlighted = LastRectangle.Contains(InputManager.MousePosition);
        }
    }
}
