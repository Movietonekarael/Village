using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Services
    {
        public class InstantiateService : MonoBehaviour
        {
            [Inject] private readonly DiContainer _diContainer;


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

            public T InstantiateObject<T>(T unityObject, Transform transform) where T : Object
            {
                return Object.Instantiate(unityObject, transform);
            }

            public GameObject InstantiateObjectWithInjections(GameObject gameObject)
            {
                return _diContainer.InstantiatePrefab(gameObject);
            }

            public GameObject InstantiateObjectWithInjections(GameObject gameObject, Transform transform)
            {
                return _diContainer.InstantiatePrefab(gameObject, transform);
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