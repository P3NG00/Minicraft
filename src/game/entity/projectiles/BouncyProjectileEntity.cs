using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Entities.Projectiles
{
    public sealed class BouncyProjectileEntity : ProjectileEntity
    {
        public BouncyProjectileEntity(Vector2 position) : base(position, Colors.Entity_BouncyProjectile) {}

        public sealed override void Update(World world)
        {
            // TODO apply gravity
            var testPosition = GetNextPosition();
            var testSides = GetSides(testPosition);
            // TODO prevent getting stuck in walls
            if (CheckHorizontalCollision(world, testSides))
                Velocity.Y *= -1f;
            if (CheckVerticalCollision(world, testSides))
                Velocity.X *= -1f;
            Position = testPosition;
        }
    }
}
