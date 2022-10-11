namespace Minicraft.Input
{
    public abstract class AbstractInput
    {
        public abstract bool PressedThisFrame { get; }

        public abstract bool ReleasedThisFrame { get; }

        public abstract bool Held { get; }
    }
}
