using Microsoft.Xna.Framework;
using Minicraft.Game.Entities.Living;
using Minicraft.Game.Inventories;
using Minicraft.Game.Worlds;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public sealed class WorldCreationScene : AbstractScene
    {
        private readonly Button _buttonBack;
        private readonly Button _buttonNew;
        private readonly Button _buttonStart;

        private World _world;
        private WorldPreview _worldPreview;

        public WorldCreationScene() : base()
        {
            _buttonBack = new(new Vector2(0.25f, 0.9f), new Point(250, 50), "Back", Colors.ThemeExit, BackToMainMenu);
            _buttonNew = new(new Vector2(0.5f, 0.9f), new Point(250, 50), "Generate New World", Colors.ThemeDefault, GenerateNewWorld);
            _buttonStart = new(new Vector2(0.75f, 0.9f), new Point(250, 50), "Start World", Colors.ThemeBlue, StartWorld);
            GenerateNewWorld();
        }

        private void BackToMainMenu() => MinicraftGame.SetScene(new MainMenuScene());

        private void GenerateNewWorld()
        {
            _world = WorldGen.GenerateWorld();
            _worldPreview = new WorldPreview(_world, new Vector2(0.5f, 0.4f));
        }

        private void StartWorld() => MinicraftGame.SetScene(new GameScene(new GameData(_world, new Inventory(), new PlayerEntity(_world))));

        public sealed override void Update(GameTime gameTime)
        {
            // update buttons
            _buttonBack.Update();
            _buttonNew.Update();
            _buttonStart.Update();
        }

        public sealed override void Draw(GameTime gameTime)
        {
            // draw buttons
            _buttonBack.Draw();
            _buttonNew.Draw();
            _buttonStart.Draw();
            // draw world preview
            _worldPreview.Draw();
        }
    }
}
