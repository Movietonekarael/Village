using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore.Boot
{
    public class Boot : MonoBehaviour
    {
        [SerializeField] private AssetReference _firstScene;

        private void Start()
        {
            Addressables.LoadSceneAsync(_firstScene, UnityEngine.SceneManagement.LoadSceneMode.Single, true);
        }
    }
}
