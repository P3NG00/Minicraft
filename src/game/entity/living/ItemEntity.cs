using Microsoft.Xna.Framework;
using MinicraftGame.Game.ItemType;

namespace MinicraftGame.Game.Entities.Living
{
    public sealed class ItemEntity : AbstractLivingEntity
    {
        private const float PICKUP_DISTANCE = 2f;
        private const float ITEM_LIFE = 1f;
        private const float ITEM_SPEED = 1f;
        private const float ITEM_RUN_MULTIPLIER = 1f;
        private const float ITEM_JUMP_VELOCITY = 1f;
        private static Vector2 ItemEntityDimensions => new(0.75f, 0.75f);

        private readonly Item _item;

        public ItemEntity(Vector2 position, Item item) : base(position, ITEM_LIFE, ItemEntityDimensions, ITEM_SPEED, ITEM_RUN_MULTIPLIER, ITEM_JUMP_VELOCITY, item.DrawData) => _item = item;

        public sealed override void Tick()
        {
            if (Minicraft.Player.GetSides().Intersects(this.GetSides()))
            {
                Minicraft.Player.Inventory.Add(_item);
                Kill();
            }
            // base call
            base.Tick();
        }
    }
}
