using Microsoft.Xna.Framework;
using Minicraft.Font;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public sealed class MainMenuScene : AbstractScene
    {
        private readonly Button _buttonWorldNew;
        private readonly Button _buttonExit;
        private readonly Button _buttonWorldContinue = null;

        public MainMenuScene() : base()
        {
            _buttonWorldNew = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), "create world", Colors.ThemeBlue, CreateNewWorld);
            _buttonExit = new Button(new Vector2(0.5f, 0.8f), new Point(120, 30), "exit", Colors.ThemeExit, MinicraftGame.EndProgram);
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
            // TODO try using smallest fontsize but scaled up to appropriate amount (fontsize36 would be fontsize12 scaled up by 3)
            Display.DrawCenteredString(FontSize._36, new Vector2(0.5f, 0.4f), MinicraftGame.TITLE, Colors.UI_Title, drawStringFunc: Display.DrawStringWithShadow);
            // draw buttons
            _buttonWorldNew.Draw();
            _buttonExit.Draw();
            _buttonWorldContinue?.Draw();
        }

        private void CreateNewWorld() => MinicraftGame.SetScene(new WorldCreationScene());

        private void LoadSavedWorld() => MinicraftGame.SetScene(new GameScene(Data.Load()));
    }
}
