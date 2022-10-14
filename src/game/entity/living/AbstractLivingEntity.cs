using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Entities.Living
{
    public abstract class AbstractLivingEntity : AbstractEntity
    {
        // constants
        private const float FALL_DISTANCE_MIN = 6f;
        private const float FALL_DAMAGE_PER_BLOCK = 0.4f;
        private const float VELOCITY_MAX = 50f;
        private const int MOVEMENT_SUBCHECKS = 16;

        public readonly float RunMultiplier;
        public readonly float JumpVelocity;

        public bool IsGrounded { get; protected set; } = false;
        public bool Running { get; protected set; } = false;
        // TODO stop running from affecting jump velocity
        public override Vector2 Velocity
        {
            get
            {
                var velocity = base.Velocity;
                if (Running)
                    velocity *= RunMultiplier;
                return velocity;
            }
        }

        private float _lastHeight;

        public AbstractLivingEntity(Vector2 position, float maxLife, Vector2 dimensions, float moveSpeed, float runMultiplier, float jumpVelocity, DrawData drawData) : base(position, maxLife, dimensions, moveSpeed, drawData)
        {
            _lastHeight = position.Y;
            RunMultiplier = runMultiplier;
            JumpVelocity = jumpVelocity;
        }

        public void Jump()
        {
            if (IsGrounded)
            {
                RawVelocity.Y = JumpVelocity;
                IsGrounded = false;
            }
        }

        public override void Update(GameData gameData)
        {
            // add velocity if falling
            if (!IsGrounded)
                RawVelocity.Y -= World.GRAVITY * World.TICK_STEP;
            // fix max velocity
            if (Velocity.Length() > VELOCITY_MAX)
                RawVelocity = Vector2.Normalize(RawVelocity) * (VELOCITY_MAX / MoveSpeed);
            // find projected new position
            var testPosition = GetNextPosition();
            // find collision points
            var testSides = GetSides(testPosition);
            // test horizontal collision
            var horizontalCollision = CheckHorizontalCollision(gameData.World, testSides);
            // test vertical collision
            var verticalCollision = false;
            if (!IsGrounded)
                verticalCollision = CheckVerticalCollision(gameData.World, testSides);
            // figure out which collision happened first
            if (verticalCollision && horizontalCollision)
            {
                var horizontalHappenedFirst = WhichCollisionFirstHorizontalElseVertical();
                Util.ActionRef<Vector2> firstCollision;
                Util.ActionRef<Vector2> secondCollision;
                Func<World, Sides, bool> secondCollisionRecheck;
                if (horizontalHappenedFirst)
                {
                    firstCollision = HandleHorizontalCollision;
                    secondCollisionRecheck = CheckVerticalCollision;
                    secondCollision = HandleVerticalCollision;
                }
                else
                {
                    firstCollision = HandleVerticalCollision;
                    secondCollisionRecheck = CheckHorizontalCollision;
                    secondCollision = HandleHorizontalCollision;
                }
                // handle first collision
                firstCollision(ref testPosition);
                // re-check second collision with new position
                if (secondCollisionRecheck(gameData.World, GetSides(testPosition)))
                    // handle second collision
                    secondCollision(ref testPosition);
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
                if (CheckOnAir(gameData.World))
                    IsGrounded = false;
                else
                    _lastHeight = Position.Y;
            }
        }

        private void HandleHorizontalCollision(ref Vector2 testPosition)
        {
            var sides = GetSides(testPosition);
            if (IsMovingLeft)
                testPosition.X = sides.Left + 1f + HalfWidth;
            else if (IsMovingRight)
                testPosition.X = sides.Right - HalfWidth;
            else
                throw new Exception("Vertical collision handled when entity's horizontal velocity is 0");
            RawVelocity.X = 0f;
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
                throw new Exception("Vertical collision handled when entity's vertical velocity is 0");
            RawVelocity.Y = 0f;
        }

        private bool WhichCollisionFirstHorizontalElseVertical()
        {
            var sides = GetSides();
            var subVelocity = (Velocity * World.TICK_STEP) / (float)MOVEMENT_SUBCHECKS;
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

        // returns true if sides are above air
        private bool CheckOnAir(World world)
        {
            var sides = GetSides();

            // catch edge of world
            if (sides.Bottom <= 0)
                return false;
            if (sides.Bottom >= World.HEIGHT)
                return true;

            for (int x = sides.Left; x <= sides.Right; x++)
            {
                var blockPoint = new Point(x, sides.Bottom - 1);
                Debug.AddAirCheck(blockPoint);
                if (!world.GetBlock(blockPoint).CanWalkThrough)
                    // collision found, not on air
                    return false;
            }
            // no collision found
            return true;
        }
    }
}
