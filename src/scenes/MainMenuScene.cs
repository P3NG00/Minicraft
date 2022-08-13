using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Minicraft.Game.Worlds;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public sealed class MainMenuScene : IScene
    {
        private readonly Button _buttonWorldNew = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), "create world", Colors.MainMenu_Button_World, Colors.MainMenu_Text_World);
        private readonly Button _buttonExit = new Button(new Vector2(0.5f, 0.8f), new Point(120, 30), "exit", Colors.MainMenu_Button_Exit, Colors.MainMenu_Text_Exit);
        private Button _buttonWorldContinue = null;

        public MainMenuScene()
        {
            _buttonWorldNew.Action = () => MinicraftGame.SetScene(new GameScene(World.GenerateWorld()));
            _buttonWorldNew.ColorBoxHighlight = Colors.MainMenu_Button_World_Highlight;
            _buttonWorldNew.ColorTextHighlight = Colors.MainMenu_Text_World_Highlight;
            _buttonExit.Action = MinicraftGame.EndProgram;
            _buttonExit.ColorBoxHighlight = Colors.MainMenu_Button_Exit_Highlight;
            _buttonExit.ColorTextHighlight = Colors.MainMenu_Text_Exit_Highlight;
            // check if save exists
            if (File.Exists(World.SAVE_FILE))
            {
                _buttonWorldContinue = new Button(new Vector2(0.5f, 0.7f), new Point(250, 50), "continue world", Colors.MainMenu_Button_World, Colors.MainMenu_Text_World);
                _buttonWorldContinue.Action = () => MinicraftGame.SetScene(new GameScene(World.Load()));
                _buttonWorldContinue.ColorBoxHighlight = Colors.MainMenu_Button_World_Highlight;
                _buttonWorldContinue.ColorTextHighlight = Colors.MainMenu_Text_World_Highlight;
            }
        }

        public void Update(GameTime gameTime)
        {
            // handle input
            if (Input.KeyFirstDown(Keys.Escape))
                MinicraftGame.EndProgram();
            // update buttons
            _buttonWorldNew.Update();
            _buttonExit.Update();
            _buttonWorldContinue?.Update();
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
            _buttonWorldContinue?.Draw();
        }
    }
}
