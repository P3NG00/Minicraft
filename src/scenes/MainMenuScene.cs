using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Minicraft.Game.Blocks;
using Minicraft.Game.Worlds;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public sealed class MainMenuScene : IScene
    {
        private readonly Button _buttonWorldNew = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), "create world", Colors.MainMenu_Button_World, Colors.MainMenu_Text_World);
        private readonly Button _buttonWorldContinue = new Button(new Vector2(0.5f, 0.7f), new Point(250, 50), "continue world", Colors.MainMenu_Button_World, Colors.MainMenu_Text_World);
        private readonly Button _buttonExit = new Button(new Vector2(0.5f, 0.8f), new Point(120, 30), "exit", Colors.MainMenu_Button_Exit, Colors.MainMenu_Text_Exit);
        private readonly bool _savedWorld;

        public MainMenuScene()
        {
            _buttonWorldNew.Action = CreateNewWorld;
            _buttonWorldNew.ColorBoxHighlight = Colors.MainMenu_Button_World_Highlight;
            _buttonWorldNew.ColorTextHighlight = Colors.MainMenu_Text_World_Highlight;
            _buttonWorldContinue.Action = LoadSavedWorld;
            _buttonWorldContinue.ColorBoxHighlight = Colors.MainMenu_Button_World_Highlight;
            _buttonWorldContinue.ColorTextHighlight = Colors.MainMenu_Text_World_Highlight;
            _buttonExit.Action = MinicraftGame.EndProgram;
            _buttonExit.ColorBoxHighlight = Colors.MainMenu_Button_Exit_Highlight;
            _buttonExit.ColorTextHighlight = Colors.MainMenu_Text_Exit_Highlight;
            _savedWorld = File.Exists(WorldGen.SAVE_FILE);
        }

        public void Update(GameTime gameTime)
        {
            // handle input
            if (Input.KeyFirstDown(Keys.Escape))
                MinicraftGame.EndProgram();
            // update buttons
            _buttonWorldNew.Update();
            _buttonExit.Update();
            if (_savedWorld)
                _buttonWorldContinue.Update();
        }

        public void Draw(GameTime gameTime)
        {
            // draw title
            var textSize = Display.FontTitle.MeasureString(MinicraftGame.TITLE);
            var x = (Display.WindowSize.X / 2f) - (textSize.X / 2f);
            var y = (Display.WindowSize.Y / 3f) - (textSize.Y / 2f);
            Display.DrawString(Display.FontTitle, new Vector2(x, y), MinicraftGame.TITLE, Colors.UI_Title);
            // draw buttons
            _buttonWorldNew.Draw();
            _buttonExit.Draw();
            if (_savedWorld)
                _buttonWorldContinue.Draw();
        }

        private void CreateNewWorld() => MinicraftGame.SetScene(new GameScene(WorldGen.GenerateWorld()));

        private void LoadSavedWorld()
        {
            var blockGrid = new BlockType[WorldGen.WorldSize.Y, WorldGen.WorldSize.X];
            using (var stream = File.OpenText(WorldGen.SAVE_FILE))
                for (int y = 0; y < WorldGen.WorldSize.Y; y++)
                    for (int x = 0; x < WorldGen.WorldSize.X; x++)
                        blockGrid[y, x] = (BlockType)stream.Read();
            MinicraftGame.SetScene(new GameScene(new World(blockGrid)));
        }
    }
}
