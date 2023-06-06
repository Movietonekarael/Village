using UnityEngine;
using Zenject;

namespace GameCore
{
    public static class InstantiateService
    {
        private static DiContainer _diContainer => DiContainerReference.Container;


        public static GameObject InstantiateObject(GameObject gameObject)
        {
            return Object.Instantiate(gameObject);
        }

        public static GameObject InstantiateObject(GameObject gameObject, Transform transform)
        {
            return Object.Instantiate(gameObject, transform);
        }

        public static GameObject InstantiateObjectWithInjections(GameObject gameObject) 
        { 
            return _diContainer.InstantiatePrefab(gameObject);
        }

        public static GameObject InstantiateObjectWithInjections(GameObject gameObject, Transform transform)
        {
            return _diContainer.InstantiatePrefab(gameObject, transform);
        }

        public static void DestroyObject(GameObject gameObject) 
        { 
            Object.Destroy(gameObject);
        }
    }
}