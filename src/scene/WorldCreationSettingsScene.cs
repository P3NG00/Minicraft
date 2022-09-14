using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds.Generation;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public sealed class WorldCreationSettingsScene : AbstractScene
    {
        private readonly WorldGen.Settings _settings;
        private readonly WorldGen.Settings _settingsOriginal;
        private readonly Button _buttonAccept;
        private readonly Button _buttonBack;

        public WorldCreationSettingsScene(WorldGen.Settings settings) : base()
        {
            _settings = settings;
            _settingsOriginal = settings.CreateCopy();
            _buttonAccept = new(new(0.5f, 3f / 7f), new(200, 50), "Accept", Colors.ThemeBlue, AcceptSettings);
            _buttonBack = new(new(0.5f, 4f / 7f), new(200, 50), "Back", Colors.ThemeExit, CancelChanges);
        }

        // passes the settings back to the world creation scene
        private void AcceptSettings() => MinicraftGame.SetScene(new WorldCreationScene(_settings));

        private void CancelChanges() => MinicraftGame.SetScene(new WorldCreationScene(_settingsOriginal));

        public override void Update(GameTime gameTime)
        {
            // update settings
            _settings.Update();
            // update accept button
            _buttonAccept.Update();
            _buttonBack.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            // draw settings
            _settings.Draw();
            // draw buttons
            _buttonAccept.Draw();
            _buttonBack.Draw();
        }
    }
}
