using GameCore.Network;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = GameCore.Network.NetworkPlayer;

namespace GameCore
{
    public sealed class PlayerSpawn : MonoBehaviour
    {
        private void Start()
        {
            ConnectPlayers();
            SubscribeForClientConnectionEvents();
        }

        private void ConnectPlayers()
        {
            var networkManager = NetworkManager.Singleton;

            if (networkManager.IsServer)
            {
                foreach (var client in networkManager.ConnectedClientsList)
                {
                    var networkPlayer = client.PlayerObject;
                    networkPlayer.GetComponent<NetworkPlayer>().ConnectNetworkPlayer();
                }
            }
            else
            {
                var networkPlayer = networkManager.LocalClient.PlayerObject;
                networkPlayer.GetComponent<NetworkPlayer>().ConnectNetworkPlayer();
            }
        }

        private void SubscribeForClientConnectionEvents()
        {
            var networkManager = NetworkManager.Singleton;

            networkManager.OnClientConnectedCallback += ConnectPlayer;
            networkManager.OnClientDisconnectCallback += DisconnectPlayer;
        }

        private void UnsubscribeForClientConnectionEvents()
        {
            var networkManager = NetworkManager.Singleton;

            networkManager.OnClientConnectedCallback -= ConnectPlayer;
            networkManager.OnClientDisconnectCallback -= DisconnectPlayer;
        }

        private void ConnectPlayer(ulong clientId)
        {
            var networkManager = NetworkManager.Singleton;
            var client = networkManager.ConnectedClients[clientId];
            var networkPlayer = client.PlayerObject;
            networkPlayer.GetComponent<NetworkPlayer>().ConnectNetworkPlayer();
        }

        private void DisconnectPlayer(ulong clientId) 
        {

        }
    }
}