using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public sealed class AddressablesSceneManager : NetworkBehaviour
{
    private static AddressablesSceneManager _singleton = null;
    public static AddressablesSceneManager Singleton => _singleton;


    private void Awake()
    {
        TrySetSingleton();
        DontDestroyOnLoad(gameObject);
    }

    private void TrySetSingleton()
    {
        if (_singleton == null)
        {
            _singleton = this;
        }
        else
        {
            Debug.LogWarning("There is already one instance of AddressablesSceneManager.");
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    public void SynchronizeScenesClientRpc()
    {
        if (IsServer) return;
        Debug.Log("Shit");
    }
}
