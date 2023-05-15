using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore
{
    public class InstantiateManager : MonoBehaviour
    {
        private static InstantiateManager _instance;

        public static InstantiateManager GetInstance(string senderName)
        {
            if (_instance is null)
                throw new Exception($"There is no InstantiateManager in the scene to attach to {senderName} script.");

            return _instance;
        }

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("There are more than one InstantiateManager in the scene.");
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        public GameObject InstantiateObject(GameObject gameObject)
        {
            return Instantiate(gameObject);
        }

        public GameObject InstantiateObject(GameObject gameObject, Transform transform)
        {
            return Instantiate(gameObject, transform);
        }

        public void DestroyObject(GameObject gameObject) 
        { 
            Destroy(gameObject);
        }
    }
}