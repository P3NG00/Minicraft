using System.Collections.Generic;
using MinicraftGame.Utils;

namespace MinicraftGame.Scenes
{
    public abstract class AbstractSimpleScene : AbstractScene
    {
        private readonly List<ISceneObject> _sceneObjects = new List<ISceneObject>();

        public void AddSceneObjects(params ISceneObject[] sceneObjects) => _sceneObjects.AddRange(sceneObjects);

        public override void Update() => _sceneObjects.ForEach(sceneObject => sceneObject?.Update());

        public override void Draw() => _sceneObjects.ForEach(sceneObject => sceneObject?.Draw());
    }
}
