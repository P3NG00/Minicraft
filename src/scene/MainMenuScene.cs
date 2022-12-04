using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Font;
using MinicraftGame.UI;
using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public sealed class MainMenuScene : AbstractScene
    {
        private readonly Button _buttonWorldNew;
        private readonly Button _buttonExit;
        private readonly Button _buttonWorldContinue = null;

        // TODO make this tick dependent, not frame dependent
        private int _updates = 0;

        public MainMenuScene() : base()
        {
            _buttonWorldNew = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), "create world", Colors.ThemeBlue, CreateNewWorld);
            _buttonExit = new Button(new Vector2(0.5f, 0.8f), new Point(120, 30), "exit", Colors.ThemeExit, Minicraft.EndProgram);
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
            var rotation = MathF.Sin(_updates++ * 0.01f) * 0.1f;
            Display.DrawCenteredString(FontSize._36, new Vector2(0.5f, 0.4f), Minicraft.TITLE, Colors.UI_Title, rotation: rotation, drawStringFunc: Display.DrawStringWithShadow);
            // draw buttons
            _buttonWorldNew.Draw();
            _buttonExit.Draw();
            _buttonWorldContinue?.Draw();
        }

        private void CreateNewWorld() => Minicraft.SetScene(new WorldCreationScene());

        private void LoadSavedWorld()
        {
            Data.Load();
            Minicraft.SetScene(new GameScene());
        }
    }
}
