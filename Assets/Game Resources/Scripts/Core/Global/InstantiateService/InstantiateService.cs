using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Services
    {
        public class InstantiateService : MonoBehaviour
        {
            [Inject] private DiContainer _diContainer;
            public DiContainer DiContainer => _diContainer;

            private static InstantiateService _singleton;
            public static InstantiateService Singleton => _singleton;


            private void Awake()
            {
                if (_singleton == null)
                {
                    _singleton = this;
                }
                else
                {
                    Debug.LogWarning("Instance of Instantiate Service already created.");
                    Destroy(this);
                }
            }

            public T CreateNewWithInjections<T>() where T : class, new()
            {
                var objectT = new T();
                _diContainer.Inject(objectT);
                return objectT;
            }

            public T InstantiateObject<T>(T unityObject) where T : Object
            {
                return Object.Instantiate(unityObject);
            }

            public T InstantiateObject<T>(T unityObject, Transform parentTransform) where T : Object
            {
                return Object.Instantiate(unityObject, parentTransform);
            }

            public T InstantiateObject<T>(T unityObject, Vector3 position, Quaternion rotation, Transform parentTransform) where T : Object
            {
                return Object.Instantiate(unityObject, position, rotation, parentTransform);
            }

            public GameObject InstantiateObjectWithInjections(GameObject gameObject)
            {
                return _diContainer.InstantiatePrefab(gameObject);
            }

            public GameObject InstantiateObjectWithInjections(GameObject gameObject, Transform parentTransform)
            {
                return _diContainer.InstantiatePrefab(gameObject, parentTransform);
            }

            public GameObject InstantiateObjectWithInjections(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parentTransform)
            {
                return _diContainer.InstantiatePrefab(gameObject, position, rotation, parentTransform);
            }

            public void DestroyObject(MonoBehaviour monoBehaviour)
            {
                DestroyObject(monoBehaviour.gameObject);
            }

            public void DestroyObject(GameObject gameObject)
            {
                Object.Destroy(gameObject);
            }
        }
    }
}