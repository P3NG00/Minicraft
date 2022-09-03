using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Entities
{
    public sealed class ProjectileEntity : Entity
    {
        private const float PROJECTILE_LIFE = 1f;
        private const float PROJECTILE_SPEED = 5f;
        private static readonly Vector2 ProjectileSize = new Vector2(0.5f);

        public ProjectileEntity(Vector2 position) : base(position, PROJECTILE_LIFE, Colors.Entity_Projectile, ProjectileSize, PROJECTILE_SPEED) => Velocity = Util.Random.NextUnitVector();

        public override void Update(World world)
        {
            // TODO kill upon collision
            Position = GetNextPosition();
        }
    }
}
