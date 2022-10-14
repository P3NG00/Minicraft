using Microsoft.Xna.Framework;
using Minicraft.Utils;

namespace Minicraft.Game.Entities.Projectiles
{
    public sealed class BouncyProjectileEntity : AbstractProjectileEntity
    {
        public BouncyProjectileEntity(Vector2 position) : base(position, new(color: Colors.Entity_BouncyProjectile)) {}

        public sealed override void Update(GameData gameData)
        {
            // TODO apply gravity
            var testPosition = GetNextPosition();
            var testSides = GetSides(testPosition);
            // TODO prevent getting stuck in walls
            if (CheckHorizontalCollision(gameData.World, testSides))
                RawVelocity.Y *= -1f;
            if (CheckVerticalCollision(gameData.World, testSides))
                RawVelocity.X *= -1f;
            Position = testPosition;
        }
    }
}
