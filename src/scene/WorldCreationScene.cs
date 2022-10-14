using Microsoft.Xna.Framework;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Game.Worlds.Generation;
using MinicraftGame.UI;
using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public sealed class WorldCreationScene : AbstractScene
    {
        private readonly Button _buttonBack;
        private readonly Button _buttonRandom;
        private readonly Button _buttonSettings;
        private readonly Button _buttonStart;

        private World _world;
        private WorldPreview _worldPreview;
        private WorldGen.Settings _settings;

        public WorldCreationScene(WorldGen.Settings settings = null) : base()
        {
            // create buttons
            _buttonBack = new(new Vector2(0.25f, 0.9f), new Point(250, 50), "Back", Colors.ThemeExit, BackToMainMenu);
            _buttonRandom = new(new Vector2(0.5f, 0.8f), new Point(250, 50), "Random World", Colors.ThemeDefault, GenerateRandomWorld);
            _buttonSettings = new(new Vector2(0.5f, 0.9f), new Point(250, 50), "Settings", Colors.ThemeDefault, OpenSettings);
            _buttonStart = new(new Vector2(0.75f, 0.9f), new Point(250, 50), "Start World", Colors.ThemeBlue, StartWorld);
            // default settings if not given
            _settings = settings ?? new();
            // create random world with settings
            GenerateRandomWorld();
        }

        private void BackToMainMenu() => Minicraft.SetScene(new MainMenuScene());

        private void GenerateRandomWorld()
        {
            _world = WorldGen.GenerateWorld(_settings);
            _worldPreview = new WorldPreview(_world, new Vector2(0.5f, 0.4f));
        }

        // passes the settings to the settings scene
        private void OpenSettings() => Minicraft.SetScene(new WorldCreationSettingsScene(_settings));

        private void StartWorld() => Minicraft.SetScene(new GameScene(new GameData(_world, new Inventory(), new PlayerEntity(_world))));

        public sealed override void Update(GameTime gameTime)
        {
            // update buttons
            _buttonBack.Update();
            _buttonRandom.Update();
            _buttonSettings.Update();
            _buttonStart.Update();
        }

        public sealed override void Draw(GameTime gameTime)
        {
            // draw buttons
            _buttonBack.Draw();
            _buttonRandom.Draw();
            _buttonSettings.Draw();
            _buttonStart.Draw();
            // draw world preview
            _worldPreview.Draw();
        }
    }
}
