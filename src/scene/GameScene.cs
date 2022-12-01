using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MinicraftGame.Font;
using MinicraftGame.Game.BlockType;
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

        // tick & frame handling variables
        private int Ticks => _ticks[0];
        private float AverageFramesPerSecond => _lastFps.Average();
        private float AverageTicksPerFrame => (float)_lastTickDifferences.Average();
        private float _tickDelta = 0f;
        private int[] _ticks = new [] {0, 0};
        private int[] _lastTickDifferences = new int[World.TICKS_PER_SECOND];
        private float[] _lastFps = new float[Display.FRAMES_PER_SECOND];
        private bool _paused = false;

        // cache
        // TODO store tick count the time the hit happened. once certain amount of seconds/ticks have passed, nullify blockhit
        private Point _blockHitPos = new(-1);
        private int _blockHits = 0;
        private Vector2 _lastMouseBlock;
        private Point _lastMouseBlockInt;
        private bool _withinReach;

        public GameScene() : base(Blocks.Air.Color)
        {
            // initialize buttons
            _buttonRespawn = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), TEXT_RESPAWN, Colors.ThemeDefault, Minicraft.Player.Respawn);
            _buttonResume = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), TEXT_RESUME, Colors.ThemeDefault, ResumeGame);
            _buttonMainMenu = new Button(new Vector2(0.5f, 0.7f), new Point(250, 50), TEXT_MAIN_MENU, Colors.ThemeExit, SaveAndMainMenu);
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
            if (!Minicraft.Player.Alive)
            {
                // update buttons
                _buttonRespawn.Update();
                _buttonMainMenu.Update();
            }
            // update for every tick step
            while (Tick())
            {
                // update debug
                Debug.Tick();
                // update world
                Minicraft.World.Tick();
                // update player
                // TODO update player outside of tick. use delta time to update player position instead of tick delta
                if (Minicraft.Player.Alive)
                    Minicraft.Player.Tick();
                // update entities
                Minicraft.World.TickEntities();
            }
        }

        public sealed override void Draw(GameTime gameTime)
        {
            UpdateFramesPerSecond((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            // update display handler
            Display.UpdateCameraOffset();
            // draw world
            Minicraft.World.Draw(_blockHitPos, _blockHits, _lastMouseBlockInt, _withinReach);
            // draw player
            if (Minicraft.Player.Alive)
                Minicraft.Player.Draw();
            // draw entities
            Minicraft.World.DrawEntities();
            // draw ui
            DrawUI();
        }

        private void ResumeGame() => _paused = false;

        private void SaveAndMainMenu()
        {
            Data.Save();
            Minicraft.SetScene(new MainMenuScene());
        }

        private bool Tick()
        {
            if (_tickDelta < World.TICK_STEP)
                return false;
            // decrement delta time by tick step
            _tickDelta -= World.TICK_STEP;
            // increment tick counter
            _ticks[0]++;
            // return success
            return true;
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
            if (Keybinds.Pause.PressedThisFrame && Minicraft.Player.Alive)
                _paused = !_paused;
            // increase/decrease time scale
            if (Keybinds.TimeScaleDecrement.PressedThisFrame)
                Debug.TimeScale -= Debug.TIME_SCALE_STEP;
            if (Keybinds.TimeScaleIncrement.PressedThisFrame)
                Debug.TimeScale += Debug.TIME_SCALE_STEP;
            // manually step time
            if (Keybinds.TimeTickStep.PressedThisFrame)
                _tickDelta += World.TICK_STEP;
            // debug
            if (Debug.Enabled && Keybinds.DebugCheckUpdates.PressedThisFrame)
                Debug.DisplayBlockChecks = !Debug.DisplayBlockChecks;
            if (Keybinds.Debug.PressedThisFrame)
                Debug.Enabled = !Debug.Enabled;
            if (Keybinds.DebugToggleGiveMode.PressedThisFrame)
                Debug.GiveBlocksElseItems = !Debug.GiveBlocksElseItems;
            // pause check
            if (_paused)
            {
                _withinReach = false;
                return;
            }
            // check hotbar num keys
            for (int i = 0; i < Inventory.SLOTS; i++)
                if (InputManager.KeyPressedThisFrame(Keys.D1 + i))
                    Minicraft.Player.Inventory.SetActiveSlot(i);
            Display.BlockScale = MathHelper.Clamp(Display.BlockScale + InputManager.ScrollWheelDelta, Display.BLOCK_SCALE_MIN, Display.BLOCK_SCALE_MAX);
            // get block position from mouse
            var mousePos = InputManager.MousePosition.ToVector2();
            mousePos.Y = Display.WindowSize.Y - mousePos.Y - 1;
            _lastMouseBlock = ((mousePos - (Display.WindowSize.ToVector2() / 2f)) / Display.BlockScale) + (Minicraft.Player.Position + new Vector2(0, Minicraft.Player.Dimensions.Y / 2f));
            _lastMouseBlockInt = _lastMouseBlock.ToPoint();
            // if player alive
            if (Minicraft.Player.Alive)
            {
                // give items if holding debug button
                if (Keybinds.Debug.Held)
                {
                    int i;
                    if (Debug.GiveBlocksElseItems)
                    {
                        for (i = 1; i < Blocks.Amount; i++)
                            if (InputManager.KeyPressedThisFrame(Keys.D0 + i))
                                Minicraft.Player.Inventory.Add(new BlockItem(Blocks.FromID(i)));
                    }
                    else
                    {
                        for (i = 1; i < Items.Amount; i++)
                            if (InputManager.KeyPressedThisFrame(Keys.D0 + i))
                                Minicraft.Player.Inventory.Add(Items.FromID(i));
                    }
                }
                // spawn projectiles
                if (Keybinds.SpawnProjectile.PressedThisFrame)
                    Minicraft.World.AddEntity(new ProjectileEntity(Minicraft.Player.Position));
                if (Keybinds.SpawnBouncyProjectile.PressedThisFrame)
                    Minicraft.World.AddEntity(new BouncyProjectileEntity(Minicraft.Player.Position));
                // test if within reach
                _withinReach = Vector2.Distance(Minicraft.Player.Center, _lastMouseBlock) <= PLAYER_REACH_RADIUS;
                if (_withinReach)
                {
                    var block = Minicraft.World.GetBlock(_lastMouseBlockInt);
                    if (block != null)
                    {
                        // handle left click (block breaking)
                        // TODO instead of clicking to break blocks, hold left click for certain amount of ticks to break block
                        if (Keybinds.MouseLeft.PressedThisFrame && block != Blocks.Air)
                            HitBlock(_lastMouseBlockInt);
                        // handle right click (block placing & interaction)
                        if (Keybinds.MouseRight.PressedThisFrame)
                            Minicraft.Player.Inventory.Use(_lastMouseBlockInt);
                        // handle middle click (spawn npc entity) // TODO remove later
                        if (Keybinds.MouseMiddle.PressedThisFrame)
                            Minicraft.World.AddEntity(new NPCEntity(_lastMouseBlock));
                    }
                }
            }
            else
                _withinReach = false;
        }

        private void HitBlock(Point hitPosition)
        {
            // if hit same block
            if (_blockHitPos == hitPosition)
                // increase hits
                _blockHits++;
            // if not same block hit start counting at new position
            else
            {
                _blockHitPos = hitPosition;
                _blockHits = 1;
            }
            // get block
            var block = Minicraft.World.GetBlock(_blockHitPos);
            // break block
            if (_blockHits >= block.HitsToBreak)
            {
                // remove block from world
                Minicraft.World.SetBlock(_blockHitPos, Blocks.Air);
                // spawn item entity where block broke
                var pos = _blockHitPos.ToVector2() + new Vector2(0.5f, 0.125f);
                var itemEntity = new ItemEntity(pos, new BlockItem(block));
                Minicraft.World.AddEntity(itemEntity);
                // reset info
                _blockHitPos = new Point(-1);
                _blockHits = 0;
            }
        }

        private void DrawUI()
        {
            // draw inventory hotbar
            Minicraft.Player.Inventory.Draw();
            // draw health bar
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (BarSize.X / 2f), Display.WindowSize.Y - Inventory.HotbarSize.Y - BarSize.Y);
            Display.Draw(drawPos, BarSize, new(color: Colors.UI_Bar));
            // adjust size to fit within bar
            drawPos += new Vector2(Util.UI_SPACER);
            var healthSize = BarSize - new Vector2(Util.UI_SPACER * 2, Util.UI_SPACER);
            // readjust size to display real health
            healthSize.X *= Minicraft.Player.Life / Minicraft.Player.MaxLife;
            Display.Draw(drawPos, healthSize, new(color: Colors.UI_Life));
            // draw health numbers on top of bar
            var healthString = $"{Minicraft.Player.Life:0.#}/{Minicraft.Player.MaxLife:0.#}";
            var textSize = FontSize._12.MeasureString(healthString);
            drawPos = new Vector2((Display.WindowSize.X / 2f) - (textSize.X / 2f), Display.WindowSize.Y - Inventory.HotbarSize.Y - 20);
            Display.DrawStringWithShadow(FontSize._12, drawPos, healthString, Colors.UI_TextLife);
            // draw death screen overlay
            if (!Minicraft.Player.Alive)
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
                    $"world_size: {World.WIDTH}x{World.HEIGHT}",
                    $"debug_blocks: {Debug.DisplayBlockChecks}",
                    $"give_mode: {(Debug.GiveBlocksElseItems ? "blocks" : "items")}",
                    $"paused: {_paused}",
                    $"time_scale: {Debug.TimeScale:0.00}",
                    $"time: {(Ticks / (float)World.TICKS_PER_SECOND):0.000}",
                    $"ticks: {Ticks} ({World.TICKS_PER_SECOND} ticks/sec)",
                    $"frames_per_second: {AverageFramesPerSecond:0.000}",
                    $"ticks_per_frame: {AverageTicksPerFrame:0.000}",
                    $"x: {Minicraft.Player.Position.X:0.000}",
                    $"y: {Minicraft.Player.Position.Y:0.000}",
                    $"block_scale: {Display.BlockScale}",
                    $"mouse_x: {_lastMouseBlock.X:0.000} ({_lastMouseBlockInt.X})",
                    $"mouse_y: {_lastMouseBlock.Y:0.000} ({_lastMouseBlockInt.Y})",
                    $"entity_count: {Minicraft.World.EntityCount}",
                    $"player_velocity: {Minicraft.Player.Velocity.Length():0.000}",
                    $"player_grounded: {Minicraft.Player.IsGrounded}",
                    $"player_running: {Minicraft.Player.Running}"})
                {
                    Display.DrawStringWithBackground(FontSize._12, drawPos, debugInfo, Colors.UI_TextDebug);
                    drawPos.Y += Util.UI_SPACER + FontSize._12.GetFont().LineSpacing;
                }
            }
        }
    }
}
