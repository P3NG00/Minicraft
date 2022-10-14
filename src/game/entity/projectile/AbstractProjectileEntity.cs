using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Entities.Projectiles
{
    public class AbstractProjectileEntity : AbstractEntity
    {
        protected const float PROJECTILE_LIFE = 1f;
        protected const float PROJECTILE_SPEED = 5f;
        private static readonly Vector2 ProjectileSize = new Vector2(0.5f);

        public AbstractProjectileEntity(Vector2 position, DrawData drawData) : base(position, PROJECTILE_LIFE, ProjectileSize, PROJECTILE_SPEED, drawData) => RawVelocity = Util.Random.NextUnitVector();
    }
}
