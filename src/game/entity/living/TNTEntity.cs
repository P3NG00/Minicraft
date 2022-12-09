using Microsoft.Xna.Framework;
using MinicraftGame.Game.Objects.BlockObject;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Entities.Living
{
    public sealed class TNTEntity : AbstractLivingEntity
    {
        public const float TNT_FUSE = 3f;
        private const float TNT_FUSE_CHAIN = 0.25f;

        private const float TNT_SPEED = 1f;
        private static Vector2 TNTSize => Vector2.One;

        public TNTEntity(Vector2 position, float fuseTime) : base(position, Minicraft.TICKS_PER_SECOND * fuseTime, TNTSize, TNT_SPEED, 0, 0, Blocks.TNT.DrawData) {}

        public override void Tick()
        {
            // base call
            base.Tick();
            // tick life
            Damage(1f);
            // explode if dead
            if (Alive)
                return;
            // play explosion sound
            Audio.Explosion.Play();
            // get position
            var position = Center.ToPoint();
            // break blocks in a 5x5 area except for corners
            var edgeLeft = position.X - 2;
            var edgeRight = position.X + 2;
            var edgeBottom = position.Y - 2;
            var edgeTop = position.Y + 2;
            for (int x = edgeLeft; x <= edgeRight; x++)
            {
                for (int y = edgeBottom; y <= edgeTop; y++)
                {
                    var isEdgeX = x == edgeLeft || x == edgeRight;
                    var isEdgeY = y == edgeBottom || y == edgeTop;
                    if (!isEdgeX || !isEdgeY)
                    {
                        var blockPos = new Point(x, y);
                        var block = Minicraft.World.GetBlock(blockPos);
                        if (block is TNTBlock tntBlock)
                            tntBlock.Ignite(blockPos, TNT_FUSE_CHAIN);
                        else
                        {
                            Minicraft.World.GetBlock(blockPos).Interact(blockPos);
                            Minicraft.World.SetBlock(blockPos, Blocks.Air);
                        }
                    }
                }
            }
        }

        protected sealed override DrawData GetDrawData()
        {
            var drawData = base.GetDrawData();
            var deltaTick = Life % Minicraft.TICKS_PER_SECOND;
            var color = deltaTick <= Minicraft.TICKS_PER_SECOND / 2f ? Color.White : drawData.Color;
            return new DrawData(drawData.Texture, color);
        }
    }
}
