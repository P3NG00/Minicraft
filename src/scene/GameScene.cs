using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MinicraftGame.Font;
using MinicraftGame.Game.BlockType;
using MinicraftGame.Game.Entities;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.Entities.Projectiles;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.ItemType;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Input;
using MinicraftGame.UI;
using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public sealed class GameScene : AbstractScene
    {
        // constants
        private const string TEXT_DEATH = "you died!";
        private const string TEXT_MAIN_MENU = "main menu";
        private const string TEXT_PAUSE = "paused...";
        private const string TEXT_RESPAWN = "respawn";
        private const string TEXT_RESUME = "resume";
        private const float PLAYER_REACH_RADIUS = 5f;

        // readonly
        private static readonly Vector2 BarSize = new Vector2(150, 30);
        private readonly Button _buttonRespawn;
        private readonly Button _buttonResume;
        private readonly Button _buttonMainMenu;
        private readonly List<AbstractEntity> _entityList = new List<AbstractEntity>();
        private readonly GameData _gameData;

        // tick & frame handling variables
        private int Ticks => _ticks[0];
        private float AverageFramesPerSecond => _lastFps.Average();
        private float AverageTicksPerFrame => (float)_lastTickDifferences.Average();
        private float _tickDelta = 0f;
        private int[] _ticks = new [] {0, 0};
        private int[] _lastTickDifferences = new int[Game.Worlds.World.TICKS_PER_SECOND];
        private float[] _lastFps = new float[Display.FRAMES_PER_SECOND];
        private bool _paused = false;

        // cache
        private BlockHit _blockHit = new BlockHit(Point.Zero, 0);
        private GridMode _gridMode = (GridMode)0; // TODO utilize
        private Vector2 _lastMouseBlock;
        private Point _lastMouseBlockInt;
        private bool _withinReach;

        // getters
        private PlayerEntity Player => _gameData.Player;
        private Inventory Inventory => Player.Inventory;
        private World World => _gameData.World;

        public GameScene(GameData gameData) : base(Blocks.Air.Color)
        {
            // initialize buttons
            _buttonRespawn = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), TEXT_RESPAWN, Colors.ThemeDefault, RespawnPlayer);
            _buttonResume = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), TEXT_RESUME, Colors.ThemeDefault, ResumeGame);
            _buttonMainMenu = new Button(new Vector2(0.5f, 0.7f), new Point(250, 50), TEXT_MAIN_MENU, Colors.ThemeExit, SaveAndMainMenu);
            // store game data
            _gameData = gameData;
        }

        public sealed override void Update(GameTime gameTime)
        {
            // handle input
            HandleInput();
            // check pause
            if (_paused)
            {
                // update pause menu
                _buttonResume.Update();
                _buttonMainMenu.Update();
                return;
            }

            UpdateTicks((float)gameTime.ElapsedGameTime.TotalSeconds * Debug.TimeScale);
            // update ui buttons
            if (!Player.Alive)
            {
                // update buttons
                _buttonRespawn.Update();
                _buttonMainMenu.Update();
            }
            // update for every tick step
            while (Tick())
            {
                // update debug
                Debug.Update();
                // update world
                World.Update();
                // update player
                if (Player.Alive)
                    Player.Update(_gameData);
                // update entities
                foreach (var entity in _entityList)
                    entity.Update(_gameData);
                // remove dead npc's
                _entityList.RemoveAll(npc => !npc.Alive);
            }
        }

        public sealed override void Draw(GameTime gameTime)
        {
            UpdateFramesPerSecond((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            // update display handler
            Display.UpdateCameraOffset(Player);
            // draw world
            World.Draw(Player, _blockHit, _lastMouseBlockInt, _withinReach);
            // draw player
            if (Player.Alive)
                Player.Draw();
            // draw entities
            foreach (var entity in _entityList)
                entity.Draw();
            // draw ui
            DrawUI();
        }

        private void ResumeGame() => _paused = false;

        private void SaveAndMainMenu()
        {
            Data.Save(_gameData);
            Minicraft.SetScene(new MainMenuScene());
        }

        private void RespawnPlayer()
        {
            // reset health
            Player.ResetLife();
            // respawn
            Player.SpawnIntoWorld(World);
        }

        private bool Tick()
        {
            if (_tickDelta >= Game.Worlds.World.TICK_STEP)
            {
                // decrement delta time by tick step
                _tickDelta -= Game.Worlds.World.TICK_STEP;
                // increment tick counter
                _ticks[0]++;
                // return success
                return true;
            }
            return false;
        }

        private void UpdateTicks(float timeThisUpdate)
        {
            // add delta time
            _tickDelta += timeThisUpdate;
            // move last tick count down
            for (int i = _lastTickDifferences.Length - 2; i >= 0; i--)
                _lastTickDifferences[i + 1] = _lastTickDifferences[i];
            // set last tick difference
            _lastTickDifferences[0] = _ticks[0] - _ticks[1];
            // update last tick count
            _ticks[1] = _ticks[0];
        }

        private void UpdateFramesPerSecond(float timeThisFrame)
        {
            // move values down
            for (int i = _lastFps.Length - 2; i >= 0; i--)
                _lastFps[i + 1] = _lastFps[i];
            // store fps value
            _lastFps[0] = 1000f / timeThisFrame;
        }

        private void HandleInput()
        {
            // toggle pause
            if (Keybinds.Pause.PressedThisFrame && Player.Alive)
                _paused = !_paused;
            // increase/decrease time scale
            if (Keybinds.TimeScaleDecrement.PressedThisFrame)
                Debug.TimeScale -= Debug.TIME_SCALE_STEP;
            if (Keybinds.TimeScaleIncrement.PressedThisFrame)
                Debug.TimeScale += Debug.TIME_SCALE_STEP;
            // manually step time
            if (Keybinds.TimeTickStep.PressedThisFrame)
                _tickDelta += Game.Worlds.World.TICK_STEP;
            // debug
            if (Debug.Enabled && Keybinds.DebugCheckUpdates.PressedThisFrame)
                Debug.DisplayBlockChecks = !Debug.DisplayBlockChecks;
            if (Keybinds.Debug.PressedThisFrame)
                Debug.Enabled = !Debug.Enabled;
            // pause check
            if (_paused)
            {
                _withinReach = false;
                return;
            }
            // check hotbar num keys
            for (int i = 0; i < Game.Inventories.Inventory.SLOTS; i++)
                if (InputManager.KeyPressedThisFrame(Keys.D1 + i))
                    Inventory.SetActiveSlot(i);
            Display.BlockScale = MathHelper.Clamp(Display.BlockScale + InputManager.ScrollWheelDelta, Display.BLOCK_SCALE_MIN, Display.BLOCK_SCALE_MAX);
            // get block position from mouse
            var mousePos = InputManager.MousePosition.ToVector2();
            mousePos.Y = Display.WindowSize.Y - mousePos.Y - 1;
            _lastMouseBlock = ((mousePos - (Display.WindowSize.ToVector2() / 2f)) / Display.BlockScale) + (Player.Position + new Vector2(0, Player.Dimensions.Y / 2f));
            _lastMouseBlockInt = _lastMouseBlock.ToPoint();
            // if player alive
            if (Player.Alive)
            {
                // give items if holding debug button
                if (Keybinds.Debug.Held)
                    for (int i = 1; i < Items.Amount; i++)
                        if (InputManager.KeyPressedThisFrame(Keys.D0 + i))
                            Inventory.Add(Items.FromID(i));
                // spawn projectiles
                if (Keybinds.SpawnProjectile.PressedThisFrame)
                    SpawnEntity(new ProjectileEntity(Player.Position));
                if (Keybinds.SpawnBouncyProjectile.PressedThisFrame)
                    SpawnEntity(new BouncyProjectileEntity(Player.Position));
                // toggle grid mode
                if (Keybinds.ToggleGridMode.PressedThisFrame)
                {
                    if ((int)_gridMode >= Enum.GetValues<GridMode>().Length - 1)
                        _gridMode = (GridMode)0;
                    else
                        _gridMode++;
                }
                // test if within reach
                _withinReach = Vector2.Distance(Player.Center, _lastMouseBlock) <= PLAYER_REACH_RADIUS;
                // catch out of bounds
                if (_withinReach &&
                    _lastMouseBlockInt.X >= 0 && _lastMouseBlockInt.X < Game.Worlds.World.WIDTH &&
                    _lastMouseBlockInt.Y >= 0 && _lastMouseBlockInt.Y < Game.Worlds.World.HEIGHT)
                {
                    var block = World.GetBlock(_lastMouseBlockInt);
                    // handle left click (block breaking)
                    // TODO instead of clicking to break blocks, hold left click for certain amount of ticks to break block
                    if (Keybinds.MouseLeft.PressedThisFrame && block != Blocks.Air)
                        _blockHit.Hit(World, Inventory, _lastMouseBlockInt, SpawnEntity);
                    // handle right click (block placing & interaction)
                    if (Keybinds.MouseRight.PressedThisFrame)
                        Inventory.Use(World, Player, _lastMouseBlockInt);
                    if (Keybinds.MouseMiddle.PressedThisFrame)
                        SpawnEntity(new NPCEntity(_lastMouseBlock));
                }
            }
            else
                _withinReach = false;
        }

        private void DrawUI()
        {
            // draw inventory hotbar
            Inventory.Draw();
            // draw health bar
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (BarSize.X / 2f), Display.WindowSize.Y - Game.Inventories.Inventory.HotbarSize.Y - BarSize.Y);
            Display.Draw(drawPos, BarSize, new(color: Colors.UI_Bar));
            // adjust size to fit within bar
            drawPos += new Vector2(Util.UI_SPACER);
            var healthSize = BarSize - new Vector2(Util.UI_SPACER * 2, Util.UI_SPACER);
            // readjust size to display real health
            healthSize.X *= Player.Life / Player.MaxLife;
            Display.Draw(drawPos, healthSize, new(color: Colors.UI_Life));
            // draw health numbers on top of bar
            var healthString = $"{Player.Life:0.#}/{Player.MaxLife:0.#}";
            var textSize = FontSize._12.MeasureString(healthString);
            drawPos = new Vector2((Display.WindowSize.X / 2f) - (textSize.X / 2f), Display.WindowSize.Y - Game.Inventories.Inventory.HotbarSize.Y - 20);
            Display.DrawStringWithShadow(FontSize._12, drawPos, healthString, Colors.UI_TextLife);
            // draw death screen overlay
            if (!Player.Alive)
            {
                Display.DrawFadedOverlay();
                // draw text
                Display.DrawCenteredString(FontSize._24, new Vector2(0.5f, 0.35f), TEXT_DEATH, Colors.UI_YouDied, drawStringFunc: Display.DrawStringWithShadow);
                // draw buttons to restart game
                _buttonRespawn.Draw();
                _buttonMainMenu.Draw();
            }
            else if (_paused)
            {
                Display.DrawFadedOverlay();
                // draw text
                Display.DrawCenteredString(FontSize._12, new Vector2(0.5f, 0.35f), TEXT_PAUSE, Colors.UI_Pause, drawStringFunc: Display.DrawStringWithShadow);
                // draw buttons
                _buttonResume.Draw();
                _buttonMainMenu.Draw();
            }
            // draw debug
            if (Debug.Enabled)
            {
                drawPos = new Vector2(Util.UI_SPACER);
                foreach (var debugInfo in new[] {
                    $"window_size: {Display.WindowSize.X}x{Display.WindowSize.Y}",
                    $"world_size: {Game.Worlds.World.WIDTH}x{Game.Worlds.World.HEIGHT}",
                    $"debug_blocks: {Debug.DisplayBlockChecks}",
                    $"paused: {_paused}",
                    $"time_scale: {Debug.TimeScale:0.00}",
                    $"time: {(Ticks / (float)Game.Worlds.World.TICKS_PER_SECOND):0.000}",
                    $"ticks: {Ticks} ({Game.Worlds.World.TICKS_PER_SECOND} ticks/sec)",
                    $"frames_per_second: {AverageFramesPerSecond:0.000}",
                    $"ticks_per_frame: {AverageTicksPerFrame:0.000}",
                    $"x: {Player.Position.X:0.000}",
                    $"y: {Player.Position.Y:0.000}",
                    $"block_scale: {Display.BlockScale}",
                    $"mouse_x: {_lastMouseBlock.X:0.000} ({_lastMouseBlockInt.X})",
                    $"mouse_y: {_lastMouseBlock.Y:0.000} ({_lastMouseBlockInt.Y})",
                    $"entity_count: {_entityList.Count}",
                    $"player_velocity: {Player.Velocity.Length():0.000}",
                    $"player_grounded: {Player.IsGrounded}",
                    $"player_running: {Player.Running}"})
                {
                    Display.DrawStringWithBackground(FontSize._12, drawPos, debugInfo, Colors.UI_TextDebug);
                    drawPos.Y += Util.UI_SPACER + FontSize._12.GetFont().LineSpacing;
                }
            }
        }

        private void SpawnEntity(AbstractEntity entity) => _entityList.Add(entity);

        private enum GridMode
        {
            Default,
            AllGrid,
            AllBlank,
        }
    }
}
