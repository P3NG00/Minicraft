using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Entities
{
    public abstract class AbstractEntity
    {
        // getters
        public float Life => _life;
        public bool Alive => _life > 0f;
        public bool IsMovingUp => Velocity.Y > 0f;
        public bool IsMovingDown => Velocity.Y < 0f;
        public bool IsMovingLeft => Velocity.X < 0f;
        public bool IsMovingRight => Velocity.X > 0f;
        public virtual Vector2 Velocity => RawVelocity * MoveSpeed;

        // readonly data
        public readonly Vector2 Dimensions;
        public readonly float HalfWidth;
        public readonly float MoveSpeed;

        private readonly DrawData _drawData;

        // public data
        public Vector2 Position;
        public Vector2 RawVelocity;
        public float MaxLife;

        // private data
        private float _life;

        public Vector2 Center => Position + new Vector2(0, Dimensions.Y / 2f);

        public AbstractEntity(Vector2 position, float maxLife, Vector2 dimensions, float moveSpeed, DrawData drawData)
        {
            Position = position;
            MaxLife = maxLife;
            _life = maxLife;
            Dimensions = dimensions;
            HalfWidth = Dimensions.X / 2f;
            MoveSpeed = moveSpeed;
            _drawData = drawData;
        }

        public void Kill() => SetLife(0);

        public void ResetLife() => SetLife(MaxLife);

        public void SetLife(float value) => _life = Math.Clamp(value, 0f, MaxLife);

        public void Damage(float amount) => _life = Math.Max(_life - amount, 0f);

        public virtual void Tick() {}

        public void Draw()
        {
            // get current screen size of entity
            var currentSize = Dimensions * Display.BlockScale;
            // find offset to reach top-left corner for draw pos
            var drawStartOffset = new Vector2(currentSize.X / 2, currentSize.Y);
            // get relative screen position
            var relativePosition = Position * Display.BlockScale;
            // flip screen y position
            relativePosition.Y *= -1;
            // find final screen draw position
            var drawPos = relativePosition - drawStartOffset;
            // draw to surface
            Display.DrawOffset(drawPos, currentSize, _drawData);
        }

        public float DistanceTo(AbstractEntity other) => Vector2.Distance(Center, other.Center);

        // TODO someday this will be replaced with Rectangles
        public struct Sides
        {
            public readonly int Top;
            public readonly int Bottom;
            public readonly int Left;
            public readonly int Right;

            public Sides(int top, int bottom, int left, int right)
            {
                Top = top;
                Bottom = bottom;
                Left = left;
                Right = right;
            }

            public bool Contains(int x, int y) => x >= Left && x <= Right && y <= Top && y >= Bottom;

            public bool Contains(Point point) => Contains(point.X, point.Y);

            public bool Intersects(Sides other) => Contains(other.Left, other.Top) || Contains(other.Right, other.Top) || Contains(other.Left, other.Bottom) || Contains(other.Right, other.Bottom);
        }

        public Sides GetSides() => GetSides(Position);

        protected Sides GetSides(Vector2 position)
        {
            // get side values as floating numbers
            var topF = position.Y + Dimensions.Y;
            var rightF = position.X + HalfWidth;
            // floor each value to get block value
            var top = topF.Floor();
            var right = rightF.Floor();
            var bottom = position.Y.Floor();
            var left = (position.X - HalfWidth).Floor();
            // fix extended edge values
            if (topF.IsInteger())
                top--;
            if (rightF.IsInteger())
                right--;
            return new Sides(top, bottom, left, right);
        }

        protected Vector2 GetNextPosition() => Position + (Velocity * World.TICK_STEP);

        // returns true if a collision happened while moving horizontally
        protected bool CheckHorizontalCollision(Sides sides)
        {
            int side;
            if (IsMovingLeft)
                side = sides.Left;
            else if (IsMovingRight)
                side = sides.Right;
            else
                // not moving, return no collision
                return false;

            // catch edges of world
            if (side < 0 || side >= World.WIDTH)
                return true;

            var bottom = Math.Max(0, sides.Bottom);
            var top = Math.Min(World.HEIGHT - 1, sides.Top);
            for (int y = bottom; y <= top; y++)
            {
                var sidePoint = new Point(side, y);
                Debug.AddCollisionCheck(sidePoint);
                if (!Minicraft.World.GetBlock(sidePoint).CanWalkThrough)
                    // found collision
                    return true;
            }

            // no collision found
            return false;
        }

        // returns true if a collision happened while moving vertically
        protected bool CheckVerticalCollision(Sides sides)
        {
            int side;
            if (IsMovingUp)
                side = sides.Top;
            else if (IsMovingDown)
                side = sides.Bottom;
            else
                // not moving, return no collision
                return false;

            // catch edges of world
            if (side < 0 || side >= World.HEIGHT)
                return true;

            var left = Math.Max(0, sides.Left);
            var right = Math.Min(World.WIDTH - 1, sides.Right);
            for (int x = left; x <= right; x++)
            {
                var sidePoint = new Point(x, side);
                Debug.AddCollisionCheck(sidePoint);
                if (!Minicraft.World.GetBlock(sidePoint).CanWalkThrough)
                    // found collision
                    return true;
            }

            // no collision found
            return false;
        }
    }
}
