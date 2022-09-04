using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Blocks
{
    public sealed class P3NG00FaceBlock : Block
    {
        public P3NG00FaceBlock(string name, Color color, int hitsToBreak, bool canWalkThrough, Texture2D texture) : base(name, color, hitsToBreak, canWalkThrough, texture) {}

        public sealed override void Interact(World world, Point position)
        {
            var sound = Audio.SingleSoundIsolated.CreateInstance();
            sound.Play();
            // base call
            base.Interact(world, position);
        }
    }
}
