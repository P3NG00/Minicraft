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
        private const int MOVEMENT_SUBCHECKS = 16;

        public readonly Vector2 Dimensions;
        public readonly float HalfWidth;

        public Vector2 Position;
        public bool IsGrounded { get; protected set; } = false;
        public Vector2 Velocity;
        public float MaxLife;

        public float Life => _life;

        public readonly float MoveSpeed;
        public readonly float JumpVelocity;

        private readonly Color _color;

        private float _lastHeight;
        private float _life;

        public bool Alive => _life > 0f;
        public bool IsMovingUp => Velocity.Y > 0f;
        public bool IsMovingDown => Velocity.Y < 0f;
        public bool IsMovingLeft => Velocity.X < 0f;
        public bool IsMovingRight => Velocity.X > 0f;

        public Entity(Vector2 position, float maxLife, Color color, Vector2 dimensions, float moveSpeed, float jumpVelocity)
        {
            Position = position;
            _lastHeight = position.Y;
            MaxLife = maxLife;
            _life = maxLife;
            _color = color;
            Dimensions = dimensions;
            HalfWidth = Dimensions.X / 2f;
            MoveSpeed = moveSpeed;
            JumpVelocity = jumpVelocity;
        }

        public void ResetHealth() => _life = MaxLife;

        public void Damage(float amount) => _life = Math.Max(_life - amount, 0f);

        public void Jump()
        {
            if (IsGrounded)
            {
                Velocity.Y = JumpVelocity;
                IsGrounded = false;
            }
        }

        public virtual void Update(World world)
        {
            // add velocity if falling // TODO add max velocity
            if (!IsGrounded)
                Velocity.Y -= World.GRAVITY * World.TICK_STEP;
            // find projected new position
            var velocityThisUpdate = Velocity * MoveSpeed * World.TICK_STEP;
            var testPosition = Position + velocityThisUpdate;
            // find collision points
            var testSides = GetSides(testPosition);
            // test horizontal collision
            var horizontalCollision = CheckHorizontalCollision(world, testSides);
            // test vertical collision
            var verticalCollision = false;
            if (!IsGrounded)
                verticalCollision = CheckVerticalCollision(world, testSides);
            // figure out which collision happened first
            if (verticalCollision && horizontalCollision)
            {
                var horizontalHappenedFirst = WhichCollisionFirstHorizontalElseVertical(velocityThisUpdate);
                if (horizontalHappenedFirst)
                {
                    // handle horizontal collision first
                    HandleHorizontalCollision(ref testPosition);
                    // re-check vertical collision with new position
                    if (CheckVerticalCollision(world, GetSides(testPosition)))
                        // handle vertical collision
                        HandleVerticalCollision(ref testPosition);
                }
                else
                {
                    // handle vertical collision first
                    HandleVerticalCollision(ref testPosition);
                    // re-check horizontal collision with new position
                    if (CheckHorizontalCollision(world, GetSides(testPosition)))
                        // handle horizontal collision
                        HandleHorizontalCollision(ref testPosition);
                }
            }
            else if (horizontalCollision)
                HandleHorizontalCollision(ref testPosition);
            else if (verticalCollision)
                HandleVerticalCollision(ref testPosition);
            // update position
            Position = testPosition;
            // if grounded, update last height
            if (IsGrounded)
            {
                var onAir = CheckOnAir(world);

                if (onAir)
                    IsGrounded = false;
                else
                    _lastHeight = Position.Y;
            }
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

        public struct Sides
        {
            public int Top;
            public int Bottom;
            public int Left;
            public int Right;

            public Sides(int top, int bottom, int left, int right)
            {
                Top = top;
                Bottom = bottom;
                Left = left;
                Right = right;
            }

            public bool Contains(Point p) => p.X >= Left && p.X <= Right && p.Y <= Top && p.Y >= Bottom;
        }

        public Sides GetSides() => GetSides(Position);

        private Sides GetSides(Vector2 position)
        {
            var top = (int)(position.Y + Dimensions.Y);
            var bottom = (int)(position.Y);
            var left = (int)(position.X - HalfWidth);
            var rightF = position.X + HalfWidth;
            var right = (int)rightF;
            if (rightF % 1f == 0f)
                right--;
            return new Sides(top, bottom, left, right);
        }

        private void HandleHorizontalCollision(ref Vector2 testPosition)
        {
            var sides = GetSides(testPosition);
            if (IsMovingLeft)
                testPosition.X = sides.Left + 1f + HalfWidth;
            else if (IsMovingRight)
                testPosition.X = sides.Right - HalfWidth;
            else
                throw new Exception("Collision handled when not moving");
            Velocity.X = 0f;
        }

        private void HandleVerticalCollision(ref Vector2 testPosition)
        {
            var sides = GetSides(testPosition);
            if (IsMovingDown)
            {
                testPosition.Y = sides.Bottom + 1f;
                IsGrounded = true;
                var fallenDistance = _lastHeight - testPosition.Y - FALL_DISTANCE_MIN;
                if (fallenDistance > 0f)
                    Damage(fallenDistance * FALL_DAMAGE_PER_BLOCK);
            }
            else if (IsMovingUp)
                testPosition.Y = sides.Top - Dimensions.Y;
            else
                throw new Exception("Collision handled when not moving");
            Velocity.Y = 0f;
        }

        private bool WhichCollisionFirstHorizontalElseVertical(Vector2 velocityThisUpdate)
        {
            var sides = GetSides();
            var subVelocity = velocityThisUpdate / (float)MOVEMENT_SUBCHECKS;
            Func<Sides, bool> crossHorizontal;
            Func<Sides, bool> crossVertical;

            if (IsMovingLeft)
                crossHorizontal = subSides => sides.Left != subSides.Left;
            else
                crossHorizontal = subSides => sides.Right != subSides.Right;

            if (IsMovingDown)
                crossVertical = subSides => sides.Bottom != subSides.Bottom;
            else
                crossVertical = subSides => sides.Top != subSides.Top;

            for (int i = 0; i < MOVEMENT_SUBCHECKS; i++)
            {
                var subPos = Position + (subVelocity * i);
                var subSides = GetSides(subPos);

                if (crossHorizontal(subSides))
                    return true;
                if (crossVertical(subSides))
                    return false;
            }

            // return true by default if reached
            return true;
        }

        // returns true if a collision happened while moving horizontally
        private bool CheckHorizontalCollision(World world, Sides sides)
        {
            int side;
            if (IsMovingLeft)
                side = sides.Left;
            else if (IsMovingRight)
                side = sides.Right;
            else
                // not moving, return no collision
                return false;

            for (int y = sides.Bottom; y <= sides.Top; y++)
            {
                var sidePoint = new Point(side, y);
                Debug.AddCollisionCheck(sidePoint);
                if (!world.GetBlockType(sidePoint).GetBlock().CanWalkThrough)
                    // found collision
                    return true;
            }

            // no collision found
            return false;
        }

        // returns true if a collision happened while moving vertically
        private bool CheckVerticalCollision(World world, Sides sides)
        {
            int side;
            if (IsMovingUp)
                side = sides.Top;
            else if (IsMovingDown)
                side = sides.Bottom;
            else
                // not moving, return no collision
                return false;

            for (int x = sides.Left; x <= sides.Right; x++)
            {
                var sidePoint = new Point(x, side);
                Debug.AddCollisionCheck(sidePoint);
                if (!world.GetBlockType(sidePoint).GetBlock().CanWalkThrough)
                    // found collision
                    return true;
            }

            // no collision found
            return false;
        }

        // returns true if sides are above air
        private bool CheckOnAir(World world)
        {
            var sides = GetSides();
            for (int x = sides.Left; x <= sides.Right; x++)
            {
                var blockPoint = new Point(x, sides.Bottom - 1);
                Debug.AddAirCheck(blockPoint);
                if (!world.GetBlockType(blockPoint).GetBlock().CanWalkThrough)
                    // collision found, not on air
                    return false;
            }
            // no collision found
            return true;
        }
    }
}
