using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Entities.Projectiles
{
    public class ProjectileEntity : AbstractProjectileEntity
    {
        public ProjectileEntity(Vector2 position) : base (position, Colors.Entity_Projectile) {}

        protected ProjectileEntity(Vector2 position, Color color) : base (position, color) {}

        public override void Update(World world)
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
