using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data
{
    public abstract class Entity
    {
        private const float FALL_DISTANCE_MIN = 4f;
        private const float FALL_DAMAGE_PER_BLOCK = 1f;

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

        public bool Alive => _life != 0f;

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
                            var fallenDistance = _lastHeight - Position.Y - FALL_DISTANCE_MIN;
                            if (fallenDistance > 0f)
                                Damage(fallenDistance * FALL_DAMAGE_PER_BLOCK);
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
                // update last height
                _lastHeight = Position.Y;
                // test walking on air
                bool onAir = true;
                for (int x = left; x <= right && onAir; x++)
                    if (!world.Block(new Point(x, bottom - 1)).CanWalkThrough)
                        onAir = false;
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
        private const float PLAYER_SPEED = 5f;
        private const float PLAYER_JUMP = 3.5f;
        private const float PLAYER_LIFE = 10f;
        private static readonly Vector2 PlayerSize = new Vector2(1.8f, 2.8f);

        public Player(World world) : base(Vector2.Zero, PLAYER_LIFE, Colors.Entity_Player, PlayerSize, PLAYER_SPEED, PLAYER_JUMP) => Respawn(world);

        public void Respawn(World world)
        {
            var playerX = (int)(world.Width / 2f);
            Position = new Vector2(playerX, Math.Max(world.GetTopBlock(playerX - 1).y, world.GetTopBlock(playerX).y) + 1);
        }

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
            // check life
            if (!Alive)
            {
                // TODO display death screen and click button to respawn
                // reset health
                ResetHealth();
                // respawn
                Respawn(world);
            }
        }
    }

    public sealed class NPC : Entity
    {
        private const float NPC_SPEED = 3f;
        private const float NPC_JUMP = 3f;
        private const float NPC_LIFE = 2f;
        private static readonly Vector2 NPCSize = new Vector2(1.5f, 2.2f);

        private const int NPC_AI_UPDATE_TICKS_MIN = World.TICKS_PER_SECOND * 3;
        private const int NPC_AI_UPDATE_TICKS_MAX = World.TICKS_PER_SECOND * 5;
        private const float NPC_AI_GOAL_DISTANCE_MIN = 0.5f;

        private int? _goalX = null;
        private int _aiUpdateTicks;

        public NPC(Vector2 position) : base(position, NPC_LIFE, Colors.Entity_NPC, NPCSize, NPC_SPEED, NPC_JUMP) => ResetAIUpdateTimer();

        private void ResetAIUpdateTimer() => _aiUpdateTicks = Util.Random.Next(NPC_AI_UPDATE_TICKS_MIN, NPC_AI_UPDATE_TICKS_MAX + 1);

        public sealed override void Update(World world)
        {
            // decrement update ticks
            _aiUpdateTicks--;
            // test update
            if (_aiUpdateTicks == 0)
            {
                _goalX = _goalX.HasValue ? null : (int?)Util.Random.Next(world.Width);
                ResetAIUpdateTimer();
            }
            // test goal
            if (_goalX.HasValue)
            {
                // if goal reached
                if (Math.Abs(Position.X - _goalX.Value) <= NPC_AI_GOAL_DISTANCE_MIN)
                {
                    _goalX = null;
                    ResetAIUpdateTimer();
                }
                else
                    Velocity.X = Position.X < _goalX.Value ? 1f : -1f;
            }
            else
                Velocity.X = 0f;
            // base call
            base.Update(world);
        }
    }
}
