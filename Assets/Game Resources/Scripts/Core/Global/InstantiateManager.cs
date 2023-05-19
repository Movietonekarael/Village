using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class InstantiateManager : MonoBehaviour
    {
        [Inject] private readonly DiContainer _diContainer;


        public GameObject InstantiateObject(GameObject gameObject)
        {
            return Instantiate(gameObject);
        }

        public GameObject InstantiateObject(GameObject gameObject, Transform transform)
        {
            return Instantiate(gameObject, transform);
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
            Destroy(gameObject);
        }
    }
}