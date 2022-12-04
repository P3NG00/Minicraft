using Microsoft.Xna.Framework;
using MinicraftGame.Game.BlockType;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Entities.Living
{
    public sealed class TNTEntity : AbstractLivingEntity
    {
        private const float TNT_FUSE = 3f;
        private const float TNT_SPEED = 1f;
        private static Vector2 TNTSize => Vector2.One;

        public TNTEntity(Vector2 position, float fuseTime = TNT_FUSE) : base(position, World.TICKS_PER_SECOND * fuseTime, TNTSize, TNT_SPEED, 0, 0, Blocks.TNT.DrawData) {}

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
                        var block = Minicraft.World.GetBlock(x, y);
                        if (block is TNTBlock tntBlock)
                            tntBlock.Interact(new Point(x, y));
                        else
                            Minicraft.World.SetBlock(x, y, Blocks.Air);
                    }
                }
            }
        }

        protected sealed override DrawData GetDrawData()
        {
            var drawData = base.GetDrawData();
            var deltaTick = Life % World.TICKS_PER_SECOND;
            var color = deltaTick <= World.TICKS_PER_SECOND / 2f ? Color.White : drawData.Color;
            return new DrawData(drawData.Texture, color);
        }
    }
}
