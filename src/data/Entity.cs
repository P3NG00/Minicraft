using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data
{
    public sealed class Entity
    {
        public Vector2 Position = Vector2.Zero;
        public bool IsGrounded { get; private set; } = false;
        public Vector2 Velocity => _velocity;

        private Vector2 _velocity = Vector2.Zero;
        private Color _color;
        private Vector2 _dimensions;
        private float _moveSpeed;
        private float _jumpVelocity;

        public Entity(Color color, Vector2 dimensions, float moveSpeed, float jumpVelocity)
        {
            _color = color;
            _dimensions = dimensions;
            _moveSpeed = moveSpeed;
            _jumpVelocity = jumpVelocity;
        }

        public void Update(Input input, Display display, World world)
        {
            // set horizontal movement
            int h = 0;
            if (input.KeyHeld(Keys.A))
                h--;
            if (input.KeyHeld(Keys.D))
                h++;
            _velocity.X = h;
            // check jump
            if (input.KeyHeld(Keys.Space) && _velocity.Y == 0f)
            {
                _velocity.Y = _jumpVelocity;
                IsGrounded = false;
            }
            // add movement this tick
            Position += (_velocity * display.TickStep) * _moveSpeed;
            // test floor
            var blockPos = Position.ToPoint();
            if (world.Block(blockPos).IsAir)
            {
                if (world.Block(blockPos - Util.UpPoint).IsAir)
                    IsGrounded = false;
            }
            else
            {
                Position.Y = MathF.Ceiling(Position.Y);
                _velocity.Y = 0f;
                IsGrounded = true;
            }
            // add velocity if falling
            if (!IsGrounded)
                _velocity.Y -= world.Gravity * display.TickStep;
            // TODO take into account wall tiles
        }

        public void Draw(Display display)
        {
            // get current screen size of player
            var currentSize = _dimensions * display.BlockScale;
            // find offset to reach top-left corner for draw pos
            var drawOffset = new Vector2(currentSize.X / 2, currentSize.Y);
            // get relative screen position
            var relativePosition = Position * display.BlockScale;
            // flip screen y position
            relativePosition.Y *= -1;
            // find final screen draw position
            var drawPos = relativePosition - drawOffset - display.CameraOffset;
            // draw to surface
            display.Draw(drawPos, currentSize, _color);
        }
    }
}
