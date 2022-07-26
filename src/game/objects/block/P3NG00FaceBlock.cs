using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects.BlockObject
{
    public sealed class P3NG00FaceBlock : Block
    {
        public P3NG00FaceBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1) : base(name, hitsToBreak, canWalkThrough, drawData, id) {}

        public sealed override void Interact(Point position)
        {
            // play sound
            Audio.SingleSoundIsolated.Play();
            // base call
            base.Interact(position);
        }
    }
}
