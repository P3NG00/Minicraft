using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Entities.Projectiles
{
    public sealed class ProjectileEntity : AbstractProjectileEntity
    {
        public ProjectileEntity(Vector2 position) : base (position, new(color: Colors.Entity_Projectile)) {}

        public sealed override void Tick()
        {
            var testPosition = GetNextPosition();
            var testSides = GetSides(testPosition);
            if (CheckHorizontalCollision(testSides) || CheckVerticalCollision(testSides))
                Kill();
            else
                Position = testPosition;
        }
    }
}
