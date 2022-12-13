using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Font;
using MinicraftGame.UI;
using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public sealed class MainMenuScene : AbstractSimpleScene
    {
        // frequency of title rotation
        private const float TITLE_ROT_FREQ = 0.025f;
        // amplitude of title rotation
        private const float TITLE_ROT_AMP = 0.125f;

        public MainMenuScene() : base()
        {
            var buttonWorldNew = new Button(new Vector2(0.5f, 0.6f), new Point(250, 50), "create world", Colors.ThemeBlue, CreateNewWorld);
            var buttonExit = new Button(new Vector2(0.5f, 0.8f), new Point(120, 30), "exit", Colors.ThemeExit, Minicraft.Instance.Exit);
            Button buttonWorldContinue = null;
            if (Data.SaveExists)
                buttonWorldContinue = new Button(new Vector2(0.5f, 0.7f), new Point(250, 50), "continue world", Colors.ThemeBlue, LoadSavedWorld);
            AddSceneObjects(buttonWorldNew, buttonExit, buttonWorldContinue);
        }

        public sealed override void Draw()
        {
            // draw title
            var rotation = MathF.Sin(Minicraft.Ticks * TITLE_ROT_FREQ) * TITLE_ROT_AMP;
            Display.DrawCenteredString(FontSize._48, new Vector2(0.5f, 0.35f), Minicraft.TITLE, Colors.UI_Title, rotation: rotation, drawStringFunc: Display.DrawStringWithShadow);
            // base call
            base.Draw();
        }

        private void CreateNewWorld() => Minicraft.SetScene(new WorldCreationScene());

        private void LoadSavedWorld()
        {
            Data.Load();
            Minicraft.SetScene(new GameScene());
        }
    }
}
