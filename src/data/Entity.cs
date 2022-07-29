using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data
{
    public abstract class Entity
    {
        public Vector2 Position;
        public bool IsGrounded { get; protected set; } = false;
        public Vector2 Dimensions { get; private set; }
        public Vector2 Velocity;
        public float JumpVelocity => _jumpVelocity;

        private readonly Color _color;
        private readonly float _moveSpeed;
        private readonly float _jumpVelocity;

        public Entity(Vector2 position, Color color, Vector2 dimensions, float moveSpeed, float jumpVelocity)
        {
            Position = position;
            _color = color;
            Dimensions = dimensions;
            _moveSpeed = moveSpeed;
            _jumpVelocity = jumpVelocity;
        }

        public virtual void Update(World world)
        {
            // add velocity if falling // TODO add max velocity
            if (!IsGrounded)
                Velocity.Y -= World.GRAVITY * World.TickStep;
            // find projected new position
            var testPosition = Position + ((Velocity * World.TickStep) * _moveSpeed);
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
                if (Velocity.Y < 0f)
                {
                    // test feet blocks
                    for (int x = left; x <= right; x++)
                    {
                        if (!world.Block(new Point(x, bottom)).CanWalkThrough)
                        {
                            testPosition.Y = MathF.Ceiling(testPosition.Y);
                            Velocity.Y = 0f;
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
                            Velocity.Y = 0f;
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
            if (Velocity.X != 0f)
            {
                // left
                if (Velocity.X < 0f)
                {
                    // test left blocks
                    for (int y = bottom; y <= top; y++)
                    {
                        if (!world.Block(new Point(left, y)).CanWalkThrough)
                        {
                            testPosition.X = left + 1 + halfWidth;
                            Velocity.X = 0f;
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
                            Velocity.X = 0f;
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

    public sealed class Player : Entity
    {
        private static readonly Vector2 PlayerSize = new Vector2(1.8f, 2.8f);
        private const float PLAYER_SPEED = 5f;
        private const float PLAYER_JUMP = 3.5f;

        public Player(Vector2 position) : base(position, Colors.Player, PlayerSize, PLAYER_SPEED, PLAYER_JUMP) {}

        public sealed override void Update(World world)
        {
            // set horizontal movement
            int h = 0;
            if (Input.KeyHeld(Keys.A))
                h--;
            if (Input.KeyHeld(Keys.D))
                h++;
            Velocity.X = h;
            // check jump
            if (IsGrounded && Input.KeyHeld(Keys.Space))
            {
                Velocity.Y = JumpVelocity;
                IsGrounded = false;
            }
            // base call
            base.Update(world);
        }
    }
}
