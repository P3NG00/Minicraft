using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Entities
{
    public sealed class PlayerEntity : Entity
    {
        private const float PLAYER_SPEED = 5f;
        private const float PLAYER_JUMP = 3.5f;
        private const float PLAYER_LIFE = 10f;
        private static readonly Vector2 PlayerSize = new Vector2(1.8f, 2.8f);

        public PlayerEntity(World world) : base(Vector2.Zero, PLAYER_LIFE, Colors.Entity_Player, PlayerSize, PLAYER_SPEED, PLAYER_JUMP) => Respawn(world);

        public void Respawn(World world)
        {
            var playerX = (int)(world.Width / 2f);
            Position = new Vector2(playerX, Math.Max(world.GetTop(playerX - 1).y, world.GetTop(playerX).y) + 1);
        }

        public sealed override void Update(World world)
        {
            // set horizontal movement
            int h = 0;
            if (Input.KeyHeld(Keys.A))
                h--;
            if (Input.KeyHeld(Keys.D))
                h++;
            Velocity.X = h;
            // check jump
            if (IsGrounded && Input.KeyHeld(Keys.Space))
                Jump();
            // base call
            base.Update(world);
            // check life
            if (!Alive)
            {
                // TODO display death screen and click button to respawn
                // reset health
                ResetHealth();
                // respawn
                Respawn(world);
            }
        }
    }
}
