using Microsoft.Xna.Framework;
using Minicraft.Utils;

namespace Minicraft.Game.Entities.Projectiles
{
    public sealed class ProjectileEntity : AbstractProjectileEntity
    {
        public ProjectileEntity(Vector2 position) : base (position, new(color: Colors.Entity_Projectile)) {}

        public sealed override void Update(GameData gameData)
        {
            var testPosition = GetNextPosition();
            var testSides = GetSides(testPosition);
            if (CheckHorizontalCollision(gameData.World, testSides) || CheckVerticalCollision(gameData.World, testSides))
                Kill();
            else
                Position = testPosition;
        }
    }
}
