namespace Minicraft.Game.Worlds.Generation
{
    public interface IWorldGenSetting
    {
        void Increment();

        void Decrement();

        void Update();

        void Draw();
    }
}
