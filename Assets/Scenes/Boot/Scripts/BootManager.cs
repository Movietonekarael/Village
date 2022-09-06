using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameCore.Boot
{
#if MONOCACHE
    public class BootManager : MonoCache
#else
    public class BootManager : MonoBehaviour
#endif

    {

        public void LoadTestLevel()
        {
            //var handle =
            Addressables.LoadSceneAsync("TestScene", UnityEngine.SceneManagement.LoadSceneMode.Single, true);
        }



        public void Exit()
        {
            Application.Quit();
        }



    }
}

