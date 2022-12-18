using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects
{
    public abstract class ObjectManager<T> where T : GameObject
    {
        private T[] _objects;

        protected abstract T[] ObjectArray { get; }

        public ObjectManager(ref ObjectManager<T> instance) => this.SingletonCheck(ref instance);

        public void Initialize()
        {
            InstantiateObjects();
            _objects = ObjectArray;
            CheckIDs();
        }

        protected abstract void InstantiateObjects();

        private void CheckIDs()
        {
            for (int i = 0; i < ObjectAmount; i++)
                if (_objects[i].ID != i)
                    throw new System.Exception($"ID mismatch: {_objects[i].Name} has ID {_objects[i].ID} but is at index {i}.");
        }

        public int ObjectAmount => _objects.Length;

        public T ObjectFromID(int i) => _objects[i];
    }
}
