using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace GameCore
{
    namespace Network
    {
        public class NetworkPlayer : NetworkBehaviour
        {
            private void Awake()
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}