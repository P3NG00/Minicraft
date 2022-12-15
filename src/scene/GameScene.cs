using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MinicraftGame.Font;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.Entities.Projectiles;
using MinicraftGame.Game.GUI;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.Objects.BlockObject;
using MinicraftGame.Game.Objects.ItemObject;
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

        public static GameScene Instance => _instance;
        public static Slot CursorSlot = new();

        private static GameScene _instance;

        private Vector2 HealthBarSize => new Vector2(150, 30);

        // readonly
        private readonly GUIHotbar _hotbar = new GUIHotbar();
        private readonly Button _buttonRespawn;
        private readonly Button _buttonResume;
        private readonly Button _buttonMainMenu;

        // cache
        // TODO store tick count the time the hit happened. once certain amount of seconds/ticks have passed, nullify blockhit
        private Point _blockHitPos = new(-1);
        private int _blockHits = 0;
        private Vector2 _lastMouseBlock;
        private Point _lastMouseBlockInt;
        private bool _withinReach;
        private bool _paused = false;

        // TODO add a parralax background somehow

        public GameScene() : base(Blocks.Air.Color)
        {
            this.SingletonCheck(ref _instance);
            // initialize buttons
            var buttonSize = new Point(250, 50);
            _buttonRespawn = new Button(new Vector2(0.5f, 0.6f), buttonSize, TEXT_RESPAWN, Colors.ThemeDefault, Minicraft.Player.Respawn);
            _buttonResume = new Button(new Vector2(0.5f, 0.6f), buttonSize, TEXT_RESUME, Colors.ThemeDefault, ResumeGame);
            _buttonMainMenu = new Button(new Vector2(0.5f, 0.7f), buttonSize, TEXT_MAIN_MENU, Colors.ThemeExit, SaveAndMainMenu);
        }

        public sealed override void Update()
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
            // update ui buttons
            if (!Minicraft.Player.Alive)
            {
                // update buttons
                _buttonRespawn.Update();
                _buttonMainMenu.Update();
                return;
            }
            // update hotbar
            _hotbar.Update();
        }

        public sealed override void Tick()
        {
            if (_paused)
                return;
            // update debug
            Debug.Tick();
            // update world
            Minicraft.World.Tick();
            // update player
            if (Minicraft.Player.Alive)
                Minicraft.Player.Tick();
            // update entities
            Minicraft.World.TickEntities();
        }

        public sealed override void Draw()
        {
            // focus camera around player
            Display.UpdateCameraOffset(Minicraft.Player.Center);
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
            Debug.TimeScale = 1f;
        }

        private void HandleInput()
        {
            // toggle pause
            if (Keybinds.Pause.PressedThisFrame && Minicraft.Player.Alive)
                Util.Toggle(ref _paused);
            // increase/decrease time scale
            if (Keybinds.TimeScaleDecrement.PressedThisFrame)
                Debug.TimeScale -= Debug.TIME_SCALE_STEP;
            if (Keybinds.TimeScaleIncrement.PressedThisFrame)
                Debug.TimeScale += Debug.TIME_SCALE_STEP;
            // manually step time
            if (Keybinds.TimeTickStep.PressedThisFrame)
                Minicraft.AddTick();
            // debug
            if (Debug.Enabled && Keybinds.DebugCheckUpdates.PressedThisFrame)
                Util.Toggle(ref Debug.DisplayBlockChecks);
            if (Keybinds.Debug.PressedThisFrame)
                Util.Toggle(ref Debug.Enabled);
            if (Keybinds.DebugToggleGiveMode.PressedThisFrame)
                Util.Toggle(ref Debug.GiveBlocksElseItems);
            // pause check
            if (_paused)
            {
                _withinReach = false;
                return;
            }
            // check hotbar num keys
            for (int i = 0; i < Inventory.SLOTS_WIDTH; i++)
                if (InputManager.KeyPressedThisFrame(Keys.D1 + i))
                    _hotbar.ActiveSlot = i;
            var scroll = InputManager.ScrollWheelDelta;
            if (scroll != 0)
                Display.BlockScale = MathHelper.Clamp(Display.BlockScale + scroll, Display.BLOCK_SCALE_MIN, Display.BLOCK_SCALE_MAX);
            // get block position from mouse
            var mousePos = InputManager.MousePosition.ToVector2();
            mousePos.Y = Display.WindowSize.Y - mousePos.Y - 1;
            _lastMouseBlock = ((mousePos - (Display.WindowSize.ToVector2() / 2f)) / Display.BlockScale) + (Minicraft.Player.Position + new Vector2(0, Minicraft.Player.Dimensions.Y / 2f));
            _lastMouseBlockInt = _lastMouseBlock.ToPoint();
            // if player alive
            if (!Minicraft.Player.Alive)
            {
                _withinReach = false;
                return;
            }
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
            {
                AbstractProjectileEntity entity = Keybinds.Shift.Held ? new BouncyProjectileEntity(Minicraft.Player.Position) : new ProjectileEntity(Minicraft.Player.Position);
                Minicraft.World.AddEntity(entity);
            }
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
                        Minicraft.Player.Inventory.Use(_hotbar.ActiveSlot, _lastMouseBlockInt);
                    // handle middle click (spawn npc entity) // TODO remove later
                    if (Keybinds.MouseMiddle.PressedThisFrame)
                        Minicraft.World.AddEntity(new NPCEntity(_lastMouseBlock));
                }
            }
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
            _hotbar.Draw();
            // draw health bar
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (HealthBarSize.X / 2f), Display.WindowSize.Y - GUIHotbar.HotbarSize.Y - HealthBarSize.Y);
            Display.Draw(drawPos, HealthBarSize, new(color: Colors.UI_Bar));
            // adjust size to fit within bar
            drawPos += new Vector2(Util.UI_SPACER);
            var healthSize = HealthBarSize - new Vector2(Util.UI_SPACER * 2, Util.UI_SPACER);
            // readjust size to display real health
            healthSize.X *= Minicraft.Player.Life / Minicraft.Player.MaxLife;
            Display.Draw(drawPos, healthSize, new(color: Colors.UI_Life));
            // draw health numbers on top of bar
            var healthString = $"{Minicraft.Player.Life:0.#}/{Minicraft.Player.MaxLife:0.#}";
            var textSize = FontSize._12.MeasureString(healthString);
            drawPos = new Vector2((Display.WindowSize.X / 2f) - (textSize.X / 2f), Display.WindowSize.Y - GUIHotbar.HotbarSize.Y - 20);
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
                // draw center point
                Display.DrawCentered(new Vector2(0.5f), new Vector2(6), new(color: Colors.Debug_CenterPoint));
                // draw ui info
                drawPos = new Vector2(Util.UI_SPACER);
                foreach (var debugInfo in new[] {
                    $"window_size: {Display.WindowSize.X}x{Display.WindowSize.Y}",
                    $"world_size: {World.WIDTH}x{World.HEIGHT}",
                    $"debug_blocks: {Debug.DisplayBlockChecks}",
                    $"give_mode: {(Debug.GiveBlocksElseItems ? "blocks" : "items")}",
                    $"paused: {_paused}",
                    $"time_scale: {Debug.TimeScale:0.00}",
                    $"time: {(Minicraft.Ticks / (float)Minicraft.TICKS_PER_SECOND):0.000}",
                    $"ticks: {Minicraft.Ticks} ({Minicraft.TICKS_PER_SECOND} ticks/sec)",
                    $"frames_per_second: {Minicraft.AverageFramesPerSecond:0.000}",
                    $"ticks_per_frame: {Minicraft.AverageTicksPerFrame:0.000}",
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
