using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Services
    {
        public class InstantiateService : MonoBehaviour
        {
            [Inject] private readonly DiContainer _diContainer;


            public GameObject InstantiateObject(GameObject gameObject)
            {
                return Object.Instantiate(gameObject);
            }

            public GameObject InstantiateObject(GameObject gameObject, Transform transform)
            {
                return Object.Instantiate(gameObject, transform);
            }

            public GameObject InstantiateObjectWithInjections(GameObject gameObject)
            {
                return _diContainer.InstantiatePrefab(gameObject);
            }

            public GameObject InstantiateObjectWithInjections(GameObject gameObject, Transform transform)
            {
                return _diContainer.InstantiatePrefab(gameObject, transform);
            }

            public void DestroyObject(GameObject gameObject)
            {
                Object.Destroy(gameObject);
            }
        }
    }
}