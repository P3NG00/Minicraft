using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Minicraft.Utils
{
    public static class Audio
    {
        public static SoundEffect SingleSoundIsolated { get; private set; }

        public static void Initialize(ContentManager content)
        {
            SingleSoundIsolated = content.Load<SoundEffect>("single_sound_isolated");
        }
    }
}
