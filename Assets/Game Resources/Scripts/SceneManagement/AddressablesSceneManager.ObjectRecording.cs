using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SceneManagement
{
    public sealed partial class AddressablesSceneManager
    {
        private readonly Hashtable _scenesForRecordedGameObjects = new();
        private readonly List<HashSet<GameObject>> _networkGameObjects = new();

        public void AddNetworkObject(GameObject gameObject)
        {
            var scene = gameObject.scene;
            if (scene == null) return;

            if (!_scenesForRecordedGameObjects.Contains(scene))
            {
                _networkGameObjects.Add(new HashSet<GameObject>());
                var newSceneId = _networkGameObjects.Count - 1;
                _scenesForRecordedGameObjects.Add(scene, newSceneId);
            }

            var sceneId = (int)_scenesForRecordedGameObjects[scene];
            _networkGameObjects[sceneId].Add(gameObject);
        }
    }
}