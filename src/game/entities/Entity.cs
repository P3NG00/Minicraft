using System;
using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game
{
    public abstract class Entity
    {
        private const float FALL_DISTANCE_MIN = 4f;
        private const float FALL_DAMAGE_PER_BLOCK = 0.5f;

        public Vector2 Position;
        public bool IsGrounded { get; protected set; } = false;
        public Vector2 Dimensions { get; private set; }
        public Vector2 Velocity;
        public float MaxLife;

        public float Life => _life;

        public readonly float MoveSpeed;
        public readonly float JumpVelocity;

        private readonly Color _color;

        private float _lastHeight;
        private float _life;

        public bool Alive => _life > 0f;

        public Entity(Vector2 position, float maxLife, Color color, Vector2 dimensions, float moveSpeed, float jumpVelocity)
        {
            Position = position;
            _lastHeight = position.Y;
            MaxLife = maxLife;
            _life = maxLife;
            _color = color;
            Dimensions = dimensions;
            MoveSpeed = moveSpeed;
            JumpVelocity = jumpVelocity;
        }

        public void ResetHealth() => _life = MaxLife;

        public void Damage(float amount) => _life = Math.Max(_life - amount, 0f);

        public void Jump()
        {
            Velocity.Y = JumpVelocity;
            IsGrounded = false;
        }

        public virtual void Update(World world)
        {
            // add velocity if falling // TODO add max velocity
            if (!IsGrounded)
                Velocity.Y -= World.GRAVITY * World.TickStep;
            // find projected new position
            var testPosition = Position + ((Velocity * World.TickStep) * MoveSpeed);
            // find collision points
            var top = (int)(testPosition.Y + Dimensions.Y);
            var bottom = (int)(testPosition.Y);
            var halfWidth = Dimensions.X / 2f;
            var left = (int)(testPosition.X - halfWidth);
            var right = (int)(testPosition.X + halfWidth);
            // test horizontal collision
            if (Velocity.X != 0f)
            {
                bool blocked = false;
                var isMovingLeft = Velocity.X < 0f;
                var side = isMovingLeft ? left : right;
                var sideOffset = isMovingLeft ? left + 1 + halfWidth : right - halfWidth;
                // check side blocks
                for (int y = bottom; y <= top && !blocked; y++)
                {
                    var sidePoint = new Point(side, y);
                    Debug.AddCollisionCheck(sidePoint);
                    if (!world.GetBlockType(sidePoint).GetBlock().CanWalkThrough)
                        blocked = true;
                }
                if (blocked)
                {
                    // fix position
                    testPosition.X = sideOffset;
                    Velocity.X = 0f;
                }
            }
            // update bound values
            left = (int)(testPosition.X - halfWidth);
            var rightF = testPosition.X + halfWidth;
            right = (int)rightF;
            // decrement right by one if whole number to avoid extended hitbox in walls
            if (rightF % 1f == 0f)
                right--;
            // entity not grounded, vertical velocity is not zero
            if (!IsGrounded)
            {
                var blocked = false;
                var isMovingDown = Velocity.Y < 0f;
                var side = isMovingDown ? bottom : top;
                for (int x = left; x <= right && !blocked; x++)
                {
                    var sidePoint = new Point(x, side);
                    Debug.AddCollisionCheck(sidePoint);
                    if (!world.GetBlockType(sidePoint).GetBlock().CanWalkThrough)
                        blocked = true;
                }
                if (blocked)
                {
                    Velocity.Y = 0f;
                    if (isMovingDown)
                    {
                        testPosition.Y = (float)Math.Ceiling(testPosition.Y);
                        IsGrounded = true;
                        var fallenDistance = _lastHeight - testPosition.Y - FALL_DISTANCE_MIN;
                        if (fallenDistance > 0f)
                            Damage(fallenDistance * FALL_DAMAGE_PER_BLOCK);
                    }
                    else
                        testPosition.Y = top - Dimensions.Y;
                }
            }
            // entity grounded
            else
            {
                // test walking on air
                bool onAir = true;
                for (int x = left; x <= right && onAir; x++)
                {
                    var blockPoint = new Point(x, bottom - 1);
                    Debug.AddCollisionCheck(blockPoint);
                    if (!world.GetBlockType(blockPoint).GetBlock().CanWalkThrough)
                        onAir = false;
                }
                if (onAir)
                    IsGrounded = false;
            }
            // update position
            Position = testPosition;
            // if grounded, update last height
            if (IsGrounded)
                _lastHeight = Position.Y;
        }

        public void Draw()
        {
            // get current screen size of entity
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
