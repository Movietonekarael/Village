using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameCore.Boot
{
    public class BootManager : MonoBehaviour
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

