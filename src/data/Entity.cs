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

        public void Update(World world)
        {
            // set horizontal movement
            int h = 0;
            if (Input.KeyHeld(Keys.A))
                h--;
            if (Input.KeyHeld(Keys.D))
                h++;
            _velocity.X = h;
            // check jump
            if (IsGrounded && Input.KeyHeld(Keys.Space))
            {
                _velocity.Y = _jumpVelocity;
                IsGrounded = false;
            }
            // add velocity if falling // TODO add max velocity
            else if (!IsGrounded)
                _velocity.Y -= World.GRAVITY * World.TickStep;
            // find projected new position
            var testPosition = Position + ((_velocity * World.TickStep) * _moveSpeed);
            // find collision points
            var top = (int)(testPosition.Y + Dimensions.Y);
            var bottom = (int)(testPosition.Y);
            var halfWidth = Dimensions.X / 2f;
            var left = (int)(testPosition.X - halfWidth);
            var right = (int)(testPosition.X + halfWidth);
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
            // player grounded
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
            // update bound values
            top = (int)(testPosition.Y + Dimensions.Y);
            bottom = (int)(testPosition.Y);
            left = (int)(testPosition.X - halfWidth);
            right = (int)(testPosition.X + halfWidth);
            // test horizontal collision
            if (_velocity.X != 0f)
            {
                // left
                if (_velocity.X < 0f)
                {
                    // test left blocks
                    for (int y = bottom; y <= top; y++)
                    {
                        if (!world.Block(new Point(left, y)).CanWalkThrough)
                        {
                            testPosition.X = left + 1 + halfWidth;
                            _velocity.X = 0f;
                            break;
                        }
                    }
                }
                // right
                else
                {
                    // test right blocks
                    for (int y = bottom; y <= top; y++)
                    {
                        if (!world.Block(new Point(right, y)).CanWalkThrough)
                        {
                            testPosition.X = right - halfWidth;
                            _velocity.X = 0f;
                            break;
                        }
                    }
                }
            }
            // update position
            Position = testPosition;
        }

        public void Draw()
        {
            // get current screen size of player
            var currentSize = Dimensions * Display.BlockScale;
            // find offset to reach top-left corner for draw pos
            var drawOffset = new Vector2(currentSize.X / 2, currentSize.Y);
            // get relative screen position
            var relativePosition = Position * Display.BlockScale;
            // flip screen y position
            relativePosition.Y *= -1;
            // find final screen draw position
            var drawPos = relativePosition - drawOffset - Display.CameraOffset;
            // draw to surface
            Display.Draw(drawPos, currentSize, _color);
        }
    }
}
