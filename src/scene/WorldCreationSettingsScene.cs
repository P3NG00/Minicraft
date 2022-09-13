using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds.Generation;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Scenes
{
    public sealed class WorldCreationSettingsScene : AbstractScene
    {
        private readonly WorldGen.Settings _settings = new();
        private readonly Button _buttonAccept;

        public WorldCreationSettingsScene(WorldGen.Settings settings) : base()
        {
            _settings = settings;
            _buttonAccept = new(new(0.5f, 0.9f), new(250, 50), "Accept", Colors.ThemeBlue, AcceptSettings);
        }

        private void AcceptSettings() => MinicraftGame.SetScene(new WorldCreationScene(_settings));

        public override void Update(GameTime gameTime)
        {
            _settings.Update();
            _buttonAccept.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            _settings.Draw();
            _buttonAccept.Draw();
        }
    }
}
