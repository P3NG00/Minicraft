using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Game.Worlds.Generation;
using MinicraftGame.UI;
using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public sealed class WorldCreationScene : AbstractSimpleScene
    {
        private readonly Vector2 _worldPreviewDrawingPosition = new Vector2(0.5f, 0.4f);

        private WorldGen.Settings _settings;
        private DrawData _worldDrawData;

        public WorldCreationScene(WorldGen.Settings settings = null) : base()
        {
            var buttonSize = new Point(250, 50);
            // create buttons
            var buttonBack = new Button(new Vector2(0.25f, 0.9f), buttonSize, "Back", Colors.ThemeExit, BackToMainMenu);
            var buttonRandom = new Button(new Vector2(0.5f, 0.8f), buttonSize, "Random World", Colors.ThemeDefault, GenerateRandomWorld);
            var buttonSettings = new Button(new Vector2(0.5f, 0.9f), buttonSize, "Settings", Colors.ThemeDefault, OpenSettings);
            var buttonStart = new Button(new Vector2(0.75f, 0.9f), buttonSize, "Start World", Colors.ThemeBlue, StartWorld);
            // default settings if not given
            _settings = settings ?? new();
            // create random world with settings
            GenerateRandomWorld();
            // add scene objects
            AddSceneObjects(buttonBack, buttonRandom, buttonSettings, buttonStart);
        }

        private void BackToMainMenu() => Minicraft.SetScene(new MainMenuScene());

        private void GenerateRandomWorld()
        {
            // generate world before it's referenced in new world preview
            Minicraft.World = WorldGen.GenerateWorld(_settings);
            // create world preview
            var worldTexture = new Texture2D(Minicraft.Instance.GraphicsDevice, World.WIDTH, World.HEIGHT);
            var data = new Color[World.WIDTH * World.HEIGHT];
            for (int y = 0; y < World.HEIGHT; y++)
            {
                var flipY = World.HEIGHT - y - 1;
                for (int x = 0; x < World.WIDTH; x++)
                {
                    var block = Minicraft.World.GetBlock(x, y);
                    var index = (flipY * World.WIDTH) + x;
                    data[index] = block.Color;
                }
            }
            worldTexture.SetData(data);
            _worldDrawData = new(worldTexture);
        }

        // passes the settings to the settings scene
        private void OpenSettings() => Minicraft.SetScene(new WorldCreationSettingsScene(_settings));

        private void StartWorld()
        {
            // instantiate player so it can be referenced in new game scene
            Minicraft.Player = new();
            Minicraft.SetScene(new GameScene());
        }

        public sealed override void Draw()
        {
            // base call
            base.Draw();
            // draw world preview
            Display.DrawCentered(_worldPreviewDrawingPosition, new Vector2(World.WIDTH, World.HEIGHT), _worldDrawData);
        }
    }
}
