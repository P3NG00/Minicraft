using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Entities.Projectile
{
    public sealed class ProjectileEntity : AbstractProjectileEntity
    {
        public ProjectileEntity(Vector2 position) : base (position, Colors.Entity_Projectile) {}

        public sealed override void Update(World world)
        {
            var testPosition = GetNextPosition();
            var testSides = GetSides(testPosition);
            if (CheckHorizontalCollision(world, testSides) || CheckVerticalCollision(world, testSides))
                Kill();
            else
                Position = testPosition;
        }
    }
}
