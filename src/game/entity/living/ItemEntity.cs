using Microsoft.Xna.Framework;
using Minicraft.Game.ItemType;
using Minicraft.Utils;

namespace Minicraft.Game.Entities.Living
{
    public sealed class ItemEntity : AbstractLivingEntity
    {
        private const float PICKUP_DISTANCE = 2f;
        private const float ITEM_LIFE = 1f;
        private const float ITEM_SPEED = 1f;
        private const float ITEM_RUN_MULTIPLIER = 1f;
        private const float ITEM_JUMP_VELOCITY = 1f;
        private static readonly Vector2 ItemEntityDimensions = new(0.75f, 0.75f);

        private readonly Item _item;

        public ItemEntity(Vector2 position, Item item) : base(position, ITEM_LIFE, ItemEntityDimensions, ITEM_SPEED, ITEM_RUN_MULTIPLIER, ITEM_JUMP_VELOCITY, item.DrawData) => _item = item;

        public sealed override void Update(GameData gameData)
        {
            if (DistanceTo(gameData.Player) <= PICKUP_DISTANCE)
            {
                gameData.Inventory.Add(_item);
                Kill();
            }
            // base call
            base.Update(gameData);
        }
    }
}
