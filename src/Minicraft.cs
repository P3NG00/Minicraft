using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game.Data;
using System.Collections.Generic;

namespace Game
{
    public class Minicraft : Microsoft.Xna.Framework.Game
    {
        // constants
        private const string TITLE = "Minicraft";

        // variables
        private Player _player;
        private List<NPC> _npcList = new List<NPC>();
        private World _world;

        public Minicraft()
        {
            Display.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / Display.FRAMES_PER_SECOND);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>((sender, eventArgs) =>
            {
                Display.WindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
                Display.UpdateWindowSize();
            });
        }

        protected override void LoadContent()
        {
            // create square for drawing
            Display.TextureSquare = new Texture2D(GraphicsDevice, 1, 1);
            Display.TextureSquare.SetData(new[] {Color.White});
            // load font
            Display.Font = Content.Load<SpriteFont>("type_writer");
            // create display handler
            Display.SpriteBatch = new SpriteBatch(GraphicsDevice);
            // create world
            _world = WorldGen.GenerateWorld();
            // create player
            _player = new Player(_world);
            // base call
            base.LoadContent();
        }

        protected override void Initialize()
        {
            // set window title
            Window.Title = TITLE;
            // set window properties
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Window.AllowAltF4 = false;
            Display.UpdateWindowSize();
            // base call
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            GameInfo.Update(_player, _world, (float)gameTime.ElapsedGameTime.TotalSeconds);
            // update for every tick step
            while (GameInfo.Tick())
            {
                // clear previously updated positions
                Debug.UpdatedPoints.Clear();
                // update input
                Input.Update();
                // handle input
                if (Input.KeyFirstDown(Keys.End))
                    Exit();
                if (Input.KeyFirstDown(Keys.Tab))
                    Display.ShowGrid = !Display.ShowGrid;
                if (Input.KeyFirstDown(Keys.F1))
                    Window.IsBorderless = !Window.IsBorderless;
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
                // update world
                _world.Update();
                // update player
                _player.Update(_world);
                // update npc's
                _npcList.ForEach(npc => npc.Update(_world));
                // update display handler
                Display.Update(_player);
            }
            // base call
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GameInfo.UpdateFramesPerSecond((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            // fill background
            GraphicsDevice.Clear(Colors.Background);
            // begin drawing
            Display.SpriteBatch.Begin();
            // draw world
            _world.Draw(_player);
            // draw player
            _player.Draw();
            // draw npc's
            _npcList.ForEach(npc => npc.Draw());
            // draw ui
            UI.Draw(_player, _world);
            // end drawing
            Display.SpriteBatch.End();
            // base call
            base.Draw(gameTime);
        }
    }
}
