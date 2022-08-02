using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data
{
    public abstract class Scene
    {
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }

    public sealed class MainMenuScene: Scene
    {
        public sealed override void Update(GameTime gameTime)
        {
            // TODO
        }

        public sealed override void Draw(GameTime gameTime)
        {
            // TODO
        }
    }

    public sealed class GameScene : Scene
    {
        private readonly Player _player;
        private readonly List<NPC> _npcList = new List<NPC>();
        private readonly World _world;

        public GameScene(World world)
        {
            _player = new Player(world);
            _world = world;
        }

        public sealed override void Update(GameTime gameTime)
        {
            GameInfo.Update(_player, _world, (float)gameTime.ElapsedGameTime.TotalSeconds);
            // handle input
            if (Input.KeyFirstDown(Keys.Tab))
                Display.ShowGrid = !Display.ShowGrid;
            if (Debug.Enabled && Input.KeyFirstDown(Keys.F11))
                Debug.TrackUpdated = !Debug.TrackUpdated;
            if (Input.KeyFirstDown(Keys.F12))
                Debug.Enabled = !Debug.Enabled;
            if (Input.KeyFirstDown(Keys.D1))
                GameInfo.CurrentBlock = Blocks.Dirt;
            if (Input.KeyFirstDown(Keys.D2))
                GameInfo.CurrentBlock = Blocks.Grass;
            if (Input.KeyFirstDown(Keys.D3))
                GameInfo.CurrentBlock = Blocks.Stone;
            if (Input.KeyFirstDown(Keys.D4))
                GameInfo.CurrentBlock = Blocks.Wood;
            if (Input.KeyFirstDown(Keys.D5))
                GameInfo.CurrentBlock = Blocks.Leaves;
            Display.BlockScale = Math.Clamp(Display.BlockScale + Input.ScrollWheel, Display.BLOCK_SCALE_MIN, Display.BLOCK_SCALE_MAX);
            // catch out of bounds
            if (GameInfo.LastMouseBlockInt.X >= 0 && GameInfo.LastMouseBlockInt.X < _world.Width &&
                GameInfo.LastMouseBlockInt.Y >= 0 && GameInfo.LastMouseBlockInt.Y < _world.Height)
            {
                bool ctrl = Input.KeyHeld(Keys.LeftControl) || Input.KeyHeld(Keys.RightControl);
                if (ctrl ? Input.ButtonLeftFirstDown() : Input.ButtonLeftDown())
                    _world.Block(GameInfo.LastMouseBlockInt) = Blocks.Air;
                if (ctrl ? Input.ButtonRightFirstDown() : Input.ButtonRightDown())
                    _world.Block(GameInfo.LastMouseBlockInt) = GameInfo.CurrentBlock;
                if (Input.ButtonMiddleFirstDown())
                    _npcList.Add(new NPC(GameInfo.LastMouseBlock));
            }
            // update for every tick step
            while (GameInfo.Tick())
            {
                // clear previously updated positions
                Debug.UpdatedPoints.Clear();
                // update world
                _world.Update();
                // update player
                _player.Update(_world);
                // update npc's
                _npcList.ForEach(npc => npc.Update(_world));
            }
        }

        public sealed override void Draw(GameTime gameTime)
        {
            GameInfo.UpdateFramesPerSecond((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            // update display handler
            Display.Update(_player);
            // draw world
            _world.Draw(_player);
            // draw player
            _player.Draw();
            // draw npc's
            _npcList.ForEach(npc => npc.Draw());
            // draw ui
            UI.Draw(_player, _world);
        }
    }
}
