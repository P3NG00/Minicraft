using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public sealed class MainMenuScene : Scene
    {
        private readonly Button _buttonWorld = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), "create world", Colors.MainMenu_Button_CreateWorld, Colors.MainMenu_Text_CreateWorld, () => MinicraftGame.SetScene(new GameScene()));
        private readonly Button _buttonExit = new Button(new Vector2(0.5f, 0.8f), new Point(120, 30), "exit", Colors.MainMenu_Button_Exit, Colors.MainMenu_Text_Exit, MinicraftGame.EndProgram);

        public MainMenuScene()
        {
            _buttonWorld.ColorBoxHighlight = Colors.MainMenu_Button_CreateWorld_Highlight;
            _buttonWorld.ColorTextHighlight = Colors.MainMenu_Text_CreateWorld_Highlight;
            _buttonExit.ColorBoxHighlight = Colors.MainMenu_Button_Exit_Highlight;
            _buttonExit.ColorTextHighlight = Colors.MainMenu_Text_Exit_Highlight;
        }

        public void Update(GameTime gameTime)
        {
            // handle input
            if (Input.KeyFirstDown(Keys.Escape))
                MinicraftGame.EndProgram();
            // update buttons
            _buttonWorld.Update();
            _buttonExit.Update();
        }

        public void Draw(GameTime gameTime)
        {
            // draw title
            var textSize = Display.FontTitle.MeasureString(MinicraftGame.TITLE);
            var x = (Display.WindowSize.X / 2f) - (textSize.X / 2f);
            var y = (Display.WindowSize.Y / 3f) - (textSize.Y / 2f);
            Display.DrawString(Display.FontTitle, new Vector2(x, y), MinicraftGame.TITLE, Colors.UI_Title);
            // draw buttons
            _buttonWorld.Draw();
            _buttonExit.Draw();
        }
    }
}
