using Microsoft.Xna.Framework;
using MinicraftGame.Game.Worlds.Generation;
using MinicraftGame.UI;
using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public sealed class WorldCreationSettingsScene : AbstractSimpleScene
    {
        private readonly WorldGenSettings _settings;
        private readonly WorldGenSettings _settingsOriginal;

        public WorldCreationSettingsScene(WorldGenSettings settings) : base()
        {
            _settings = settings;
            _settingsOriginal = settings.CreateCopy();
            var buttonSize = new Point(200, 50);
            var buttonAccept = new Button(new(0.5f, 3f / 7f), buttonSize, "Accept", Colors.ThemeBlue, AcceptSettings);
            var buttonBack = new Button(new(0.5f, 4f / 7f), buttonSize, "Back", Colors.ThemeExit, CancelChanges);
            // add scene objects
            AddSceneObjects(_settings, buttonAccept, buttonBack);
        }

        // passes new settings to world creation scene
        private void AcceptSettings() => Minicraft.SetScene(new WorldCreationScene(_settings));

        // passes original settings to world creation scene
        private void CancelChanges() => Minicraft.SetScene(new WorldCreationScene(_settingsOriginal));
    }
}
