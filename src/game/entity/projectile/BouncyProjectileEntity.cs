using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Entities.Projectiles
{
    public sealed class BouncyProjectileEntity : AbstractProjectileEntity
    {
        public BouncyProjectileEntity(Vector2 position) : base(position, new(color: Colors.Entity_BouncyProjectile)) {}

        public sealed override void Tick()
        {
            // TODO apply gravity
            var testPosition = GetNextPosition();
            var testSides = GetSides(testPosition);
            // TODO prevent getting stuck in walls
            if (CheckHorizontalCollision(testSides))
                RawVelocity.Y *= -1f;
            if (CheckVerticalCollision(testSides))
                RawVelocity.X *= -1f;
            Position = testPosition;
        }
    }
}
