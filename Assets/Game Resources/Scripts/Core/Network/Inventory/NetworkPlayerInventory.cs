using GameCore.Inventory;
using GameCore.Services;
using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Network
    {
        namespace Inventory
        {
            public sealed class NetworkPlayerInventory : DefaultNetworkBehaviour, IInventory, IMovableInventory, IDropableInventory
            {
                private PlayerInventory _playerInventory;

                [Inject] private ItemIdentificationService _itemIdentificationervice;

                public event Action<int> OnItemChanged;
                private GameItem[] _gameItems;


                protected override void AllOnNetworkSpawn()
                {
                    InstantiateService.Singleton.DiContainer.Inject(this);
                }

                protected override void OnClientNetworkSpawn()
                {
                    if (!IsOwner) return;
                    var numberOfItems = GetComponent<PlayerInventory>().InventorySize;
                    _gameItems = new GameItem[numberOfItems];

                    PlayerInventoryWrapper.Inventory = this;
                    PlayerInventoryWrapper.MovableInventory = this;
                    PlayerInventoryWrapper.DropableInventory = this;
                }

                protected override void OnServerNetworkSpawn()
                {
                    _playerInventory = GetComponent<PlayerInventory>();
                    _playerInventory.OnItemChanged += RealInventoryChanged;
                }

                protected override void OnServerNetworkDespawn()
                {
                    _playerInventory.OnItemChanged -= RealInventoryChanged;
                }

                private void RealInventoryChanged(int position)
                {
                    var item = _playerInventory.GetGameItem(position);

                    var networkItem = new NetworkGameItem(item?.Data.ItemID ?? 0u,
                                                          item?.Number ?? 0,
                                                          item is not null);
                    var clientRpcParams = NetworkConnectionService.GetClientParameters(OwnerClientId);
                    ChangeItemClientRpc(networkItem, position, clientRpcParams);
                }

                [ClientRpc]
                private void ChangeItemClientRpc(NetworkGameItem networkGameItem, int position, ClientRpcParams clientRpcParams = default)
                {
                    var gameItem = networkGameItem.DeserializeGameItem(_itemIdentificationervice);
                    _gameItems[position] = gameItem;
                    OnItemChanged?.Invoke(position);
                }

                public int GetInventorySize()
                {
                    return _gameItems.Length;
                }

                public GameItem GetGameItem(int position)
                {
                    return _gameItems[position];
                }

                public GameItem Pull(int position)
                {
                    if (IsClient)
                    {
                        PullServerRpc(position);
                    }
                    return null;
                }

                [ServerRpc]
                private void PullServerRpc(int position)
                {
                    PullOnServer(position);
                }

                public void PullOnServer(int position)
                {
                    _playerInventory.Pull(position);
                }

                public bool Push(ref GameItem item)
                {
                    if (IsClient)
                    {
                        var networkItem = new NetworkGameItem(item.Data.ItemID, item.Number);
                        PushServerRpc(networkItem);
                    }
                    return false;
                }

                [ServerRpc]
                private void PushServerRpc(NetworkGameItem networkGameItem)
                {
                    var gameItemData = _itemIdentificationervice.GetItemData(networkGameItem.Id);
                    var gameItem = new GameItem(gameItemData, networkGameItem.Number);
                    PushOnServer(ref gameItem);
                }

                public void PushOnServer(ref GameItem item)
                {
                    _playerInventory.Push(ref item);
                }

                public bool Push(ref GameItem item, int position)
                {
                    if (IsClient)
                    {
                        var networkItem = new NetworkGameItem(item.Data.ItemID, item.Number);
                        PushServerRpc(networkItem, position);
                    }
                    return false;
                }

                [ServerRpc]
                private void PushServerRpc(NetworkGameItem networkGameItem, int position)
                {
                    var gameItemData = _itemIdentificationervice.GetItemData(networkGameItem.Id);
                    var gameItem = new GameItem(gameItemData, networkGameItem.Number);
                    PushOnServer(ref gameItem, position);
                }

                private void PushOnServer(ref GameItem item, int position)
                {
                    _playerInventory.Push(ref item, position);
                }

                public void MoveItem(int fromPosition, int toPosition)
                {
                    if (!IsClient) return;
                    MoveItemServerRpc(fromPosition, toPosition);
                }

                [ServerRpc]
                private void MoveItemServerRpc(int fromPosition, int toPosition)
                {
                    MoveItemOnServer(fromPosition, toPosition);
                }

                public void MoveItemOnServer(int fromPosition, int toPosition)
                {
                    _playerInventory.MoveItem(fromPosition, toPosition);
                }

                public void DropItem(int position)
                {
                    if (!IsClient) return;
                    DropItemServerRpc(position);
                }

                [ServerRpc]
                private void DropItemServerRpc(int position)
                {
                    DropItemOnServer(position);
                }

                public void DropItemOnServer(int position)
                {
                    _playerInventory.DropItem(position);
                }
            }
        }
    }
}