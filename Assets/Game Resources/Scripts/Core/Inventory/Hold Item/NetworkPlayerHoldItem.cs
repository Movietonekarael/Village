using GameCore.Inventory;
using GameCore.Services;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Network
    {
        namespace Inventory
        {
            [RequireComponent(typeof(PlayerHoldItem))]
            [RequireComponent(typeof(PlayerDoll))]
            public sealed class NetworkPlayerHoldItem : DefaultNetworkBehaviour, IPlayerHoldItem
            {
                private IInventory _inventory;
                [Inject] private ItemIdentificationService _itemIdentificationervice;
                private PlayerHoldItem _holdItem;

                private int _currentHoldItemPosition = 0;


                protected override void AllOnNetworkSpawn()
                {
                    _holdItem = GetComponent<PlayerHoldItem>();
                    _inventory = GetComponent<PlayerDoll>().Player.PlayerInventory;
                }

                protected override void OnServerNetworkSpawn()
                {
                    NetworkManager.Singleton.OnClientConnectedCallback += SendHoldingItemInformation;
                }

                protected override void OnServerNetworkDespawn()
                {
                    NetworkManager.Singleton.OnClientConnectedCallback -= SendHoldingItemInformation;
                }

                protected override void OnClientNetworkSpawn()
                {
                    InstantiateService.Singleton.DiContainer.Inject(this);
                }

                public void SetItem(GameItem item)
                {
                    throw new System.NotImplementedException();
                }

                public void SetItem(int position)
                {
                    SetItemServerRpc(position);
                    var item = _inventory.GetGameItem(position);
                    _holdItem.SetItem(item);
                }

                [ServerRpc]
                private void SetItemServerRpc(int position)
                {
                    _currentHoldItemPosition = position;
                    var networkItem = GetNetworkItem();
                    SetItemClientRpc(networkItem);
                }

                private NetworkGameItem GetNetworkItem()
                {
                    var item = _inventory.GetGameItem(_currentHoldItemPosition);
                    var networkItem = new NetworkGameItem(item?.Data.ItemID ?? 0u,
                                                          item?.Number ?? 0,
                                                          item is not null);
                    return networkItem;
                }

                [ClientRpc]
                private void SetItemClientRpc(NetworkGameItem networkItem, ClientRpcParams clientRpcParams = default)
                {
                    var item = networkItem.DeserializeGameItem(_itemIdentificationervice);
                    _holdItem.SetItem(item);
                }

                private void SendHoldingItemInformation(ulong clientId)
                {
                    var clientRpcParams = NetworkConnectionService.GetClientParameters(clientId);
                    var networkItem = GetNetworkItem();
                    SetItemClientRpc(networkItem, clientRpcParams);
                }
            }
        }
    }
}