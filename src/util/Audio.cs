using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace MinicraftGame.Utils
{
    public static class Audio
    {
        public static SoundEffect Explosion { get; private set; }
        public static SoundEffect SingleSoundIsolated { get; private set; }

        public static void Initialize(ContentManager content)
        {
            Explosion = Load("explosion");
            SingleSoundIsolated = Load("single_sound_isolated");

            // local func
            SoundEffect Load(string name) => content.Load<SoundEffect>(name);
        }
    }
}
