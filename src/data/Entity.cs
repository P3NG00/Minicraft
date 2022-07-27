using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data
{
    public sealed class Entity
    {
        public Vector2 Position = Vector2.Zero;
        public bool IsGrounded { get; private set; } = false;
        public Vector2 Dimensions { get; private set; }
        public Vector2 Velocity => _velocity;

        private Vector2 _velocity = Vector2.Zero;
        private Color _color;
        private float _moveSpeed;
        private float _jumpVelocity;

        public Entity(Color color, Vector2 dimensions, float moveSpeed, float jumpVelocity)
        {
            _color = color;
            Dimensions = dimensions;
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
            if (IsGrounded && input.KeyHeld(Keys.Space))
            {
                _velocity.Y = _jumpVelocity;
                IsGrounded = false;
            }
            // add velocity if falling // TODO add max velocity
            else if (!IsGrounded)
                _velocity.Y -= world.Gravity * display.TickStep;
            // find projected new position
            var testPosition = Position + ((_velocity * display.TickStep) * _moveSpeed);
            // find collision points
            var top = (int)(testPosition.Y + Dimensions.Y);
            var bottom = (int)(testPosition.Y);
            var left = (int)(testPosition.X - (Dimensions.X / 2f));
            var right = (int)(testPosition.X + (Dimensions.X / 2f));
            // player not grounded, vertical velocity is not zero
            if (!IsGrounded)
            {
                // down
                if (_velocity.Y < 0f)
                {
                    // test feet blocks
                    for (int x = left; x <= right; x++)
                    {
                        if (!world.Block(new Point(x, bottom)).CanWalkThrough)
                        {
                            testPosition.Y = MathF.Ceiling(testPosition.Y);
                            _velocity.Y = 0f;
                            IsGrounded = true;
                            break;
                        }
                    }
                }
                // up
                else
                {
                    // test head blocks
                    for (int x = left; x <= right; x++)
                    {
                        if (!world.Block(new Point(x, top)).CanWalkThrough)
                        {
                            testPosition.Y = top - Dimensions.Y;
                            _velocity.Y = 0f;
                            break;
                        }
                    }
                }
            }
            else
            {
                // test walking on air
                bool onAir = true;
                for (int x = left; x <= right && onAir; x++)
                {
                    if (!world.Block(new Point(x, bottom - 1)).CanWalkThrough)
                        onAir = false;
                }
                if (onAir)
                    IsGrounded = false;
            }
            // test horizontal collision
            if (_velocity.X != 0f)
            {
                // left
                if (_velocity.X < 0f)
                {
                    // TODO
                    Console.WriteLine("Left Horizontal Collision Not Implemented");
                }
                // right
                else
                {
                    // TODO
                    Console.WriteLine("Right Horizontal Collision Not Implemented");
                }
            }

            // update position
            Position = testPosition;
        }

        public void Draw(Display display)
        {
            // get current screen size of player
            var currentSize = Dimensions * display.BlockScale;
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
