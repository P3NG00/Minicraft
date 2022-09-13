namespace Minicraft.Game.Worlds.Generation
{
    public interface IWorldGenSetting
    {
        public void Increment();

        public void Decrement();

        public void Update();

        public void Draw();
    }
}