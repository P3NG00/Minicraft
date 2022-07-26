using Microsoft.Xna.Framework;
using MinicraftGame.Input;

namespace MinicraftGame.Utils
{
    public abstract class AbstractHighlightable
    {
        protected Rectangle LastRectangle { get; private set; }
        protected Vector2 RelativeCenter { get; private set; }
        protected bool Highlighted { get; private set; }

        protected abstract Rectangle GetRectangle { get; }

        protected bool Clicked => Keybinds.MouseLeft.ReleasedThisFrame && Highlighted;

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

        public virtual void Update()
        {
            LastRectangle = GetRectangle;
            Highlighted = LastRectangle.Contains(InputManager.MousePosition);
        }
    }
}
