using Microsoft.Xna.Framework;
using Minicraft.Font;
using Minicraft.Game.Entities;
using Minicraft.Game.Inventories;
using Minicraft.Game.Worlds;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public sealed class MainMenuScene : Scene
    {
        private readonly Button _buttonWorldNew = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), "create world", Colors.ThemeBlue, CreateNewWorld);
        private readonly Button _buttonExit = new Button(new Vector2(0.5f, 0.8f), new Point(120, 30), "exit", Colors.ThemeExit, MinicraftGame.EndProgram);
        private readonly Button _buttonWorldContinue = null;

        public MainMenuScene() : base(Colors.Background)
        {
            // check if save exists
            if (Data.SaveExists)
                _buttonWorldContinue = new Button(new Vector2(0.5f, 0.7f), new Point(250, 50), "continue world", Colors.ThemeBlue, LoadSavedWorld);
        }

        public sealed override void Update(GameTime gameTime)
        {
            // update buttons
            _buttonWorldNew.Update();
            _buttonExit.Update();
            _buttonWorldContinue?.Update();
        }

        public sealed override void Draw(GameTime gameTime)
        {
            // draw title
            Display.DrawCenteredString(FontSize._36, new Vector2(0.5f, 0.4f), MinicraftGame.TITLE, Colors.UI_Title, Display.DrawStringWithShadow);
            // draw buttons
            _buttonWorldNew.Draw();
            _buttonExit.Draw();
            _buttonWorldContinue?.Draw();
        }

        // TODO create a 'world creator' scene with adjustable settings and an 'update changes' button to generate a world to preview your changes
        private static void CreateNewWorld()
        {
            var world = WorldGen.GenerateWorld();
            var gameData = new GameData(world, new Inventory(), new PlayerEntity(world));
            MinicraftGame.SetScene(new GameScene(gameData));
        }

        private static void LoadSavedWorld() => MinicraftGame.SetScene(new GameScene(Data.Load()));
    }
}
