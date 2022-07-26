using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public abstract class AbstractSimpleScene : AbstractScene
    {
        private ISceneObject[] _sceneObjects;

        public void SetSceneObjects(params ISceneObject[] sceneObjects) => _sceneObjects = sceneObjects;

        public override void Update() => _sceneObjects.ForEach(sceneObject => sceneObject?.Update());

        public override void Draw() => _sceneObjects.ForEach(sceneObject => sceneObject?.Draw());
    }
}
