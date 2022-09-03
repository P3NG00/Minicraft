using Microsoft.Xna.Framework;
using Minicraft.Utils;

namespace Minicraft.Game.Entities.Projectiles
{
    public class AbstractProjectileEntity : Entity
    {
        protected const float PROJECTILE_LIFE = 1f;
        protected const float PROJECTILE_SPEED = 5f;
        private static readonly Vector2 ProjectileSize = new Vector2(0.5f);

        public AbstractProjectileEntity(Vector2 position, Color color) : base(position, PROJECTILE_LIFE, color, ProjectileSize, PROJECTILE_SPEED) => Velocity = Util.Random.NextUnitVector();
    }
}
