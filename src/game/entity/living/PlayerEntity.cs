using Microsoft.Xna.Framework;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Input;
using MinicraftGame.Scenes;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Entities.Living
{
    public sealed class PlayerEntity : AbstractLivingEntity
    {
        private const float PLAYER_SPEED = 5f;
        private const float PLAYER_JUMP = 3f;
        private const float PLAYER_RUN_MULT = 1.5f;
        private const float PLAYER_LIFE = 10f;
        private static Vector2 PlayerSize => new Vector2(14f / 8f, 21f / 8f);

        public readonly Inventory Inventory;

        public PlayerEntity(Vector2 position, Inventory inventory = null) : base(position, PLAYER_LIFE, PlayerSize, PLAYER_SPEED, PLAYER_RUN_MULT, PLAYER_JUMP, new(color: Colors.Entity_Player)) => Inventory = inventory ?? new();

        public PlayerEntity() : this(Vector2.Zero) => SpawnIntoWorld();

        protected sealed override void OnDeath()
        {
            // TODO implement way to drop items from inventory
            // TODO then implement method to drop all items at once from inventory
            // Minicraft.Player.Inventory.DropAll();
            GameScene.Instance.CloseInventoryGUI();
        }

        public void SpawnIntoWorld() => Position = Minicraft.World.GetSpawnPosition().ToVector2();

        public void Respawn()
        {
            ResetLife();
            SpawnIntoWorld();
        }

        public sealed override void Tick()
        {
            // set horizontal movement
            RawVelocity.X = 0;
            if (Keybinds.MoveLeft.Held)
                RawVelocity.X--;
            if (Keybinds.MoveRight.Held)
                RawVelocity.X++;
            // check running
            Running = Keybinds.Shift.Held;
            // check jump
            if (Keybinds.Jump.Held)
                Jump();
            // base call
            base.Tick();
        }
    }
}
