using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data.Scenes
{
    public sealed class MainMenuScene : Scene
    {
        private readonly Button _buttonWorld = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), "create world", Colors.Button_CreateWorld, () => Minicraft.SetScene(new GameScene()));
        private readonly Button _buttonExit = new Button(new Vector2(0.5f, 0.8f), new Point(120, 30), "exit", Colors.Button_Exit, Minicraft.EndProgram);

        public void Update(GameTime gameTime)
        {
            // handle input
            if (Input.KeyFirstDown(Keys.Escape))
                Minicraft.EndProgram();
            // update buttons
            _buttonWorld.Update();
            _buttonExit.Update();
        }

        public void Draw(GameTime gameTime)
        {
            // draw title
            var textSize = Display.FontTitle.MeasureString(Minicraft.TITLE);
            var x = (Display.WindowSize.X / 2f) - (textSize.X / 2f);
            var y = (Display.WindowSize.Y / 3f) - (textSize.Y / 2f);
            Display.DrawString(Display.FontTitle, new Vector2(x, y), Minicraft.TITLE, Colors.UI_Title);
            // draw buttons
            _buttonWorld.Draw();
            _buttonExit.Draw();
        }
    }
}
