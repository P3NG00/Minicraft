namespace MinicraftGame.Game.Objects
{
    public abstract class ObjectManager<T> where T : GameObject
    {
        private T[] _objects;

        public void Initialize()
        {
            InstantiateObjects();
            AddObjects(ref _objects);
            CheckIDs();
        }

        protected abstract void InstantiateObjects();

        protected abstract void AddObjects(ref T[] objects);

        private void CheckIDs()
        {
            for (int i = 0; i < ObjectAmount; i++)
                if (_objects[i].ID != i)
                    throw new System.Exception($"ID mismatch: {_objects[i].Name} has ID {_objects[i].ID} but is at index {i}.");
        }

        protected int ObjectAmount => _objects.Length;

        protected T ObjectFromID(int i) => _objects[i];
    }
}
